using Xunit;
using Moq;
using AuthServer.Users;
using AuthServer.Security;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace AuthServer.Tests.Users
{
    public class UsersServiceTest
    {
        private readonly UsersService _service;
        private readonly Mock<IUsersRepository> _usersRepositoryMock;
        private readonly Mock<IRoleRepository> _rolesRepositoryMock;
        private readonly Mock<IJwt> _jwtMock;
        private readonly Mock<ILogger<UsersService>> _loggerMock = new(); 


        public UsersServiceTest()
        {
            _usersRepositoryMock = new Mock<IUsersRepository>(MockBehavior.Loose);
            _rolesRepositoryMock = new Mock<IRoleRepository>(MockBehavior.Loose);
            _jwtMock = new Mock<IJwt>(MockBehavior.Loose);

            _service = new UsersService(_jwtMock.Object, _usersRepositoryMock.Object, _rolesRepositoryMock.Object, 
                _loggerMock.Object);
        }

        [Fact]
        public void Delete_ShouldReturnFalse_IfUserDoesNotExist()
        {
            _usersRepositoryMock.Setup(repo => repo.GetById(It.IsAny<long>())).Returns((User)null);

            var result = _service.Delete(1);

            Assert.False(result);
            _usersRepositoryMock.Verify(repo => repo.Delete(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public void Delete_ShouldReturnTrue_IfUserIsDeleted()
        {
            var user = new User { ID = 1, Name = "Test User" };
            _usersRepositoryMock.Setup(repo => repo.GetById(user.ID)).Returns(user);

            var result = _service.Delete(user.ID);

            Assert.True(result);
            _usersRepositoryMock.Verify(repo => repo.Delete(user), Times.Once);
        }

        [Fact]
        public void Delete_ShouldThrowException_IfUserIsLastAdmin()
        {
            var user = new User { ID = 1, Name = "Admin User", Roles = new HashSet<Role> { new Role { Name = "ADMIN" } } };
            _usersRepositoryMock.Setup(repo => repo.GetById(user.ID)).Returns(user);
            _usersRepositoryMock.Setup(repo => repo.FindAll()).Returns(new List<User> { user });

            var exception = Assert.Throws<BadRequestException>(() => _service.Delete(user.ID));
            Assert.Equal("Cannot delete the last system admin!", exception.Message);
        }
    }
}
