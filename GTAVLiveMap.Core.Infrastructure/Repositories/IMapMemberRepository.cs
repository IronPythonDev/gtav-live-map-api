﻿using GTAVLiveMap.Domain.Entities;
using GTAVLiveMap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Infrastructure.Repositories
{
    public interface IMapMemberRepository : IRepository<MapMember , Guid>
    {
        public Task<IList<MapMember>> GetByMapId(Guid id, int limit = int.MaxValue, int offset = int.MaxValue);
        public Task<IList<MapMember>> GetByUserId(int id, int limit = int.MaxValue, int offset = int.MaxValue);
        public Task<MapMember> GetByMapAndUserId(Guid mapId , int userId);
        public Task<MapMember> GetByMapIdAndUserIdAndScopes(Guid mapId, int userId, IList<MapScopeNameEnum> scopes);
    }
}