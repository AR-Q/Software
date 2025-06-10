using IAM.Application.AuthenticationService.Interfaces;
using IAM.Application.common.Repositores;
using IAM.Application.Common.Hash;
using IAM.Contracts.User;
using IAM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAM.Application.AuthenticationService
{
    public class UserUpdateService : IUserUpdateService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHasher _hasher;

        public UserUpdateService(IUserRepository userRepository, IHasher hasher)
        {
            _userRepository = userRepository;
            _hasher = hasher;
        }

        public async Task<UserUpdateVM> handle(UserUpdateVM userUpdate)
        {
            User user = await _userRepository.Find(userUpdate.UserId);

            user.FirstName = userUpdate.FirstName;
            user.LastName = userUpdate.LastName;
            user.Password = _hasher.Hash(userUpdate.Password);
            user.NationalID = userUpdate.NationalID;
            user.Phone = userUpdate.Phone;
            user.Email = userUpdate.Email;
            user.BirthDate = userUpdate.BirthDate;

            User updatedUser = await _userRepository.Update(user);

            if(updatedUser == null)
            {
                userUpdate.UserId = -1;
            }

            userUpdate.Password = "*";

            return userUpdate;
        }
    }
}
