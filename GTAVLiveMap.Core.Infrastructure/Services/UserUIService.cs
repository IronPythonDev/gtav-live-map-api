using GTAVLiveMap.Core.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Services
{
    public class UserUIService : IUserUIService
    {
        public UserUIService(
            IMapMemberRepository mapMemberRepository)
        {
            MapMemberRepository = mapMemberRepository;
        }

        IMapMemberRepository MapMemberRepository { get; }

        public async Task<object> GetUserMenuByMemberIdAndMapId(int memberId , Guid mapId)
        {
            var member = await MapMemberRepository.GetByMapAndUserId(mapId, memberId);

            if (member == null) return null;

            IList<string> scopes = member.Scopes.Split(';');

            IList<object> menu = new List<object>();

            menu.Add(new
            {
                label = "Details",
                icon = "pi pi-fw pi-home",
                routerLink = "details"
            });

            if (scopes.Contains("ViewAction")) 
                menu.Add(new 
                {
                    label = "Actions",
                    icon = "pi pi-fw pi-calendar",
                    routerLink = "actions"
                });

            if (scopes.Contains("ViewMembers"))
                menu.Add(new
                {
                    label = "Members",
                    icon = "pi pi-fw pi-user",
                    routerLink = "members"
                });

            if (scopes.Contains("ViewInvite"))
                menu.Add(new
                {
                    label = "Invites",
                    icon = "pi pi-fw pi-plus",
                    routerLink = "invites"
                });

            return menu;
        }
    }
}
