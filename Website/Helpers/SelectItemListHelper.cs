using Microsoft.AspNetCore.Mvc.Rendering;
using Models.Entities;
using System.Collections.Generic;

namespace Website.Helpers
{
	public static class SelectItemListHelper
	{
		public static List<SelectListItem> CreateSelectItemListFromPermissions(IEnumerable<Permission> permissions)
		{
			var selectItemList = new List<SelectListItem>();

			foreach (var permission in permissions) {
				selectItemList.Add(new SelectListItem($"{permission.Name} - {permission.Action}", permission.Id.ToString()));
			}

			return selectItemList;
		}
	}
}