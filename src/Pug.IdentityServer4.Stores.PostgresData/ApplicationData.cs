using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Pug.Application.Data;
using Pug.IdentityServer.Services.Data;

namespace Pug.IdentityServer.Stores.PostgresData
{
	public class ApplicationData : ApplicationData<IGrantDataSession>
	{
		public ApplicationData( string location, DbProviderFactory dataProvider ) : base( location, dataProvider )
		{
		}

		override  protected IEnumerable<SchemaVersion> InitializeUpgradeScripts()
		{
			return new List<SchemaVersion>()
			{
				new (
					1,
					new UpgradeScript[]
					{
						new (
								"Grant Table",
								string.Empty,
								@"create table grant(
				                    identifier character varying not null,
				                    description character varying not null default '',
				                    type character varying not null,
				                    subject character varying not null,
				                    client character varying not null,
				                    data character varying not null,
				                    expirationTimestamp timestamp without time zone,
				                    consumeTimestamp timestamp without time zone,
				                    creationTimestamp timestamp without time zone not null,
				                    registrationTimestamp timestamp with time zone not null default current_timestamp,
				                    primary key (identifier)
									);

									create index grant_type_idx on grant(type);
									create index grant_subject_idx on grant(subject);
									create index grant_client_idx on grant(client);
								",
								"drop table grant;"
						)

					}
				)
			};
		}
		
		protected override IGrantDataSession CreateApplicationDataSession( IDbConnection databaseSession, DbProviderFactory dataAccessProvider )
		{
			return new GrantDataSession( databaseSession );
		}
	}
}