using Data.Dapper.Models;
using Models.DTO;
using System.Threading.Tasks;

namespace Business.Service.EntityServices.Interfaces
{
	public interface IUserManagementService : IUserService
	{
		Task<InsertResponse<long>> Create(UpdateUserDTO updateUserDTO);

		Task<UpdateUserDTO> SelectEmptyUserView();

		Task<UpdateUserDTO> SelectUserViewById(long userId);

		Task<TransactionResponse> Update(UpdateUserDTO updateUserDTO);
	}
}