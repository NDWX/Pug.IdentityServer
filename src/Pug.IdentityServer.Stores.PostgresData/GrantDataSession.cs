using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using IdentityServer4.Models;
using Pug.Application.Data;
using Pug.IdentityServer.Services.Data;

namespace Pug.IdentityServer.Stores.PostgresData
{
	public class GrantDataSession : ApplicationDataSession, IGrantDataSession
	{
		public GrantDataSession( IDbConnection databaseSession ) : base( databaseSession )
		{
		}

		public Task InsertAsync( PersistedGrant grant )
		{
			var parameters = new
			{
				grant.Key,
				grant.Description,
				grant.Type,
				grant.SubjectId,
				grant.ClientId,
				grant.SessionId,
				grant.Data,
				grant.Expiration,
				grant.ConsumedTime,
				grant.CreationTime
			};

			return Connection.ExecuteAsync(
					@"insert into grant(identifier, description, type, subject, client, session, data, expirationTimestamp, consumeTimestamp, creationTImestamp, registrationTimestamp)
							values(@key, @description, @type, @subject, @client, @session, @data, @expiration, @consumedTime, @creationTime, current_timestamp)",
					parameters
				);
		}

		public Task<PersistedGrant> GetPersistedGrant( string identifier )
		{
			return Connection.QueryFirstAsync<PersistedGrant>(
					@"select identifier as key, description, type, subject as subjectId, client as clientId, session as sessionId,
							data, expirationTimestamp as expiration, consumeTimestamp as consumedTime, creationTimestamp as creationTime
							from grant where identifier = @identifier",
					new { identifier }
				);
		}

		public Task<IEnumerable<PersistedGrant>> GetPersistedGrants( string type, string subject, string client, string session )
		{
			return Connection.QueryAsync<PersistedGrant>(
					@"select identifier as key, description, type, subject as subjectId, client as clientId, session as sessionId,
							data, expirationTimestamp as expiration, consumeTimestamp as consumedTime, creationTimestamp as creationTime
							from grant 
                            where case when coalesce(@type, '') = '' then true else type = @type end and
                                case when coalesce(@subject, '') = '' then true else subject = @subject end and
                                case when coalesce(@client, '') = '' then true else client = @client end and
                                case when coalesce(@session, '') = '' then true else session = @session end",
					new { type, subject, client, session }
				);
		}

		public Task DeletePersistedGrant( string identifier )
		{
			return Connection.ExecuteAsync(
					@"delete from grant where identifier = @identifier",
					new { identifier }
				);
		}

		public Task RemovePersistedGrants( string type, string subject, string client, string session )
		{
			return Connection.ExecuteAsync(
					@"delete from grant 
                            where case when coalesce(@type, '') = '' then true else type = @type end and
                                case when coalesce(@subject, '') = '' then true else subject = @subject end and
                                case when coalesce(@client, '') = '' then true else client = @client end and
                                case when coalesce(@session, '') = '' then true else session = @session end",
					new { type, subject, client, session }
				);
		}
	}
}