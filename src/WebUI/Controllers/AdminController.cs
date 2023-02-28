using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
	[Route("admin")]
	[Authorize]
	public class AdminController : BaseController
	{
		
	}
}