using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ModCore.CoreApi.Entities
{
    public class ApiRoleList
    {
		[JsonProperty("roles")]
		public List<ApiRole> Roles = new List<ApiRole>();
    }

	public class ApiRole
	{
		[JsonProperty("roleid")]
		public ulong RoleId;

		[JsonProperty("rolename")]
		public string RoleName;
	}
}
