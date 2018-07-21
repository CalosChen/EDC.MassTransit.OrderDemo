using Dapper;
using Manulife.DNC.MSAD.WS.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Manulife.DNC.MSAD.WS.EventService.Repositories
{
    public class OrderEventDapperRepository : IEventRepository<IOrderEventEntity>
    {
        private string connStr;

        public OrderEventDapperRepository(string connStr)
        {
            this.connStr = connStr;
        }

        public async Task<IEnumerable<IOrderEventEntity>> GetEvents(string eventType)
        {
            using (var conn = new SqlConnection(connStr))
            {
                string sqlCommand = @"SELECT [EventID],
                                                        [EventType],
                                                        [OrderID],
                                                        [CreatedTime],
                                                        [StatusKey],
                                                        [StatusValue],
                                                        [EntityJson]
                                                        FROM [dbo].[Events]
                                                        WHERE EventType = @EventType 
                                                            AND StatusValue = @StatusValue";

                var result = await conn.QueryAsync<OrderEventEntity>(sqlCommand, param: new
                {
                    EventType = EventConstants.EVENT_TYPE_CREATE_ORDER,
                    StatusValue = EventStatusEnum.UNHANDLE
                });

                return result;
            }
        }

        public async Task<bool> UpdateEventStatus(string eventType, IOrderEventEntity orderEvent)
        {
            using (var conn = new SqlConnection(connStr))
            {
                string sqlCommand = @"UPDATE [dbo].[Events]
                                                            SET StatusValue = @StatusValue
                                                            WHERE OrderID = @OrderID
                                                                AND EventType = @EventType
                                                                AND StatusKey = @StatusKey";

                var result = await conn.ExecuteAsync(sqlCommand, param: new
                {
                    OrderID = orderEvent.OrderID,
                    EventType = EventConstants.EVENT_TYPE_CREATE_ORDER,
                    StatusKey = orderEvent.StatusKey,
                    StatusValue = EventStatusEnum.HANDLED
                });

                return result > 0;
            }
        }
    }
}
