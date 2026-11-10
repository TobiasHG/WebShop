using Microsoft.AspNetCore.Identity;
using System;
using System.Buffers.Text;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using WebShop.Models;
using WebShop.Repositories;

namespace WebShop.Services
{
    public class ResetPasswordService
    {
        private readonly ResetPasswordRepository _resetPasswordRepository;
        private readonly UserService _userService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly PassOMeterService _passOMeterService;

        public ResetPasswordService(
            ResetPasswordRepository resetPasswordRepository,
            UserService userService,
            IPasswordHasher<User> passwordHasher,
            PassOMeterService passOMeterService)
        {
            _resetPasswordRepository = resetPasswordRepository;
            _userService = userService;
            _passwordHasher = passwordHasher;
            _passOMeterService = passOMeterService;
        }

        public async Task<bool> ResetPassword(string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
            {
                return false;
            }

            if (_passOMeterService.Verify(newPassword).Any())
            {
                return false;
            }

            PasswordResetToken? resetPasswordToken = await _resetPasswordRepository.GetByHash(HashToken(token));

            if (resetPasswordToken == null || resetPasswordToken.ExpiresAt < DateTime.UtcNow)
            {
                return false;
            }

            User? user = await _userService.GetById(resetPasswordToken.UserId);

            if (user == null)
            {
                return false;
            }

            string newPasswordHash = _passwordHasher.HashPassword(user, newPassword);

            await _userService.UpdatePassword(user.Id, newPasswordHash);

            return true;
        }

        public string GenerateResetToken()
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(32);

            return Base64Url.EncodeToString(bytes);
        }

        public string HashToken(string token)
        {
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(hash);
        }

        public bool VerifyToken(string token, string hashedToken)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(hashedToken))
                return false;

            byte[] computed = Encoding.UTF8.GetBytes(HashToken(token));
            byte[] stored = Encoding.UTF8.GetBytes(hashedToken);

            return CryptographicOperations.FixedTimeEquals(computed, stored);
        }
        public async Task<string?> RequestPasswordReset(string email)
        {
            User? user = await _userService.GetByEmail(email);

            if (user == null)
            {
                return null;
            }

            if (!await _userService.HasPermission(user.Id, Permissions.CanResetPassword))
            {
                return null;
            }

            string token = GenerateResetToken();
            string tokenHash = HashToken(token);

            PasswordResetToken resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                TokenHash = tokenHash,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            await _resetPasswordRepository.DeleteByUserId(user.Id);
            await _resetPasswordRepository.Add(resetToken);

            string resetLink = $"/Auth/ResetPassword?token={Uri.EscapeDataString(token)}";

            return resetLink;

            // TODO: send reset link som email istedet..
        }
    }
}