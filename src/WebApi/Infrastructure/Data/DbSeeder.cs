using Bogus;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using WebApi.Domain.Departments;
using WebApi.Domain.Users;

namespace WebApi.Infrastructure.Data
{
    internal class DbSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public DbSeeder(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public void Seed()
        {
            if (_context.Departments.Any())
                return;

            var counter = 1;
            const string password = "123@qwe";

            var hrDepartment = CreateDepartment(DepartmentType.HumanResources, "HR Department");
            var hrUsers = GenerateUsers(hrDepartment.Id, 5);
            _context.Departments.Add(hrDepartment);
            _context.Users.AddRange(hrUsers);

            var developmentDepartment = CreateDepartment(DepartmentType.Development, "Development Department");
            var developmentUsers = GenerateUsers(developmentDepartment.Id, 25);
            _context.Departments.Add(developmentDepartment);
            _context.Users.AddRange(developmentUsers);

            var devopsDepartment = CreateDepartment(DepartmentType.DevOps, "DevOps Department");
            var devopsUsers = GenerateUsers(devopsDepartment.Id, 5);
            _context.Departments.Add(devopsDepartment);
            _context.Users.AddRange(devopsUsers);

            var managementDepartment = CreateDepartment(DepartmentType.Management, "Management Department");
            var managementUsers = GenerateUsers(managementDepartment.Id, 5);
            _context.Departments.Add(managementDepartment);
            _context.Users.AddRange(managementUsers);

            _context.SaveChanges();

            Department CreateDepartment(DepartmentType type, string name) => new() { Id = Guid.NewGuid(), Type = type, Name = name };

            List<User> GenerateUsers(Guid departmentId, int count)
            {
                Faker faker = new();
                List<User> users = new();

                for (var i = 0; i < count; i++)
                {
                    var username = $"user{counter}@sample.com";
                    var bytes = new byte[20];
                    RandomNumberGenerator.Fill(bytes);
                    var user = new User
                    {
                        DepartmentId = departmentId,
                        FirstName = faker.Name.FirstName(),
                        LastName = faker.Name.LastName(),
                        BirthDate = DateTime.Now,
                        Email = username,
                        UserName = username,
                        EmailConfirmed = true,
                        NormalizedEmail = username.ToUpper(),
                        NormalizedUserName = username.ToUpper(),
                        SecurityStamp = Base32.ToBase32(bytes),
                        Id = Guid.NewGuid().ToString(),
                    };
                    user.PasswordHash = _passwordHasher.HashPassword(user, password);
                    users.Add(user);

                    counter++;
                }

                return users;
            }
        }
    }

    internal static class Base32
    {
        private static readonly string _base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        public static string ToBase32(byte[] input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var sb = new StringBuilder();
            for (var offset = 0; offset < input.Length;)
            {
                var numCharsToOutput = GetNextGroup(input, ref offset, out var a, out var b, out var c, out var d, out var e, out var f, out var g, out var h);

                sb.Append((numCharsToOutput >= 1) ? _base32Chars[a] : '=');
                sb.Append((numCharsToOutput >= 2) ? _base32Chars[b] : '=');
                sb.Append((numCharsToOutput >= 3) ? _base32Chars[c] : '=');
                sb.Append((numCharsToOutput >= 4) ? _base32Chars[d] : '=');
                sb.Append((numCharsToOutput >= 5) ? _base32Chars[e] : '=');
                sb.Append((numCharsToOutput >= 6) ? _base32Chars[f] : '=');
                sb.Append((numCharsToOutput >= 7) ? _base32Chars[g] : '=');
                sb.Append((numCharsToOutput >= 8) ? _base32Chars[h] : '=');
            }

            return sb.ToString();
        }

        public static byte[] FromBase32(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            input = input.TrimEnd('=').ToUpperInvariant();
            if (input.Length == 0)
            {
                return new byte[0];
            }

            var output = new byte[input.Length * 5 / 8];
            var bitIndex = 0;
            var inputIndex = 0;
            var outputBits = 0;
            var outputIndex = 0;
            while (outputIndex < output.Length)
            {
                var byteIndex = _base32Chars.IndexOf(input[inputIndex]);
                if (byteIndex < 0)
                {
                    throw new FormatException();
                }

                var bits = Math.Min(5 - bitIndex, 8 - outputBits);
                output[outputIndex] <<= bits;
                output[outputIndex] |= (byte)(byteIndex >> (5 - (bitIndex + bits)));

                bitIndex += bits;
                if (bitIndex >= 5)
                {
                    inputIndex++;
                    bitIndex = 0;
                }

                outputBits += bits;
                if (outputBits >= 8)
                {
                    outputIndex++;
                    outputBits = 0;
                }
            }
            return output;
        }

        // returns the number of bytes that were output
        private static int GetNextGroup(byte[] input, ref int offset, out byte a, out byte b, out byte c, out byte d, out byte e, out byte f, out byte g, out byte h)
        {
            uint b1, b2, b3, b4, b5;

            int retVal;
            switch (input.Length - offset)
            {
                case 1: retVal = 2; break;
                case 2: retVal = 4; break;
                case 3: retVal = 5; break;
                case 4: retVal = 7; break;
                default: retVal = 8; break;
            }

            b1 = (offset < input.Length) ? input[offset++] : 0U;
            b2 = (offset < input.Length) ? input[offset++] : 0U;
            b3 = (offset < input.Length) ? input[offset++] : 0U;
            b4 = (offset < input.Length) ? input[offset++] : 0U;
            b5 = (offset < input.Length) ? input[offset++] : 0U;

            a = (byte)(b1 >> 3);
            b = (byte)(((b1 & 0x07) << 2) | (b2 >> 6));
            c = (byte)((b2 >> 1) & 0x1f);
            d = (byte)(((b2 & 0x01) << 4) | (b3 >> 4));
            e = (byte)(((b3 & 0x0f) << 1) | (b4 >> 7));
            f = (byte)((b4 >> 2) & 0x1f);
            g = (byte)(((b4 & 0x3) << 3) | (b5 >> 5));
            h = (byte)(b5 & 0x1f);

            return retVal;
        }
    }
}