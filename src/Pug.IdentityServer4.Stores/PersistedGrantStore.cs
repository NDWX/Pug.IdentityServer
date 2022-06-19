using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Pug.Application.Data;
using Pug.IdentityServer.Services.Data;

namespace Pug.IdentityServer.Stores
{
	public class PersistedGrantStore<TD> : IPersistedGrantStore
		where TD : class, IGrantDataSession, IApplicationDataSession
	{
		private readonly IApplicationData<TD> _applicationData;

		public PersistedGrantStore( IApplicationData<TD> applicationData )
		{
			_applicationData = applicationData ?? throw new ArgumentNullException( nameof(applicationData) );
		}

		void Validate( PersistedGrant grant )
		{
			if( string.IsNullOrWhiteSpace( grant.Key ) )
				throw new ArgumentException( "Grant key must be specified", nameof(grant) );
			
			if( string.IsNullOrWhiteSpace( grant.Type ) )
				throw new ArgumentException( "Grant type must be specified", nameof(grant) );
			
			if( string.IsNullOrWhiteSpace( grant.ClientId ) )
				throw new ArgumentException( "Grant client identifier must be specified", nameof(grant) );
		}
		
		public virtual Task StoreAsync( PersistedGrant grant )
		{
			if( grant is null ) throw new ArgumentNullException( nameof(grant) );
			
			Validate( grant );
			
			return _applicationData.PerformAsync(
					( dataSession, context ) =>
					{
						return dataSession.InsertAsync( context.grant );
					},
					new { @this = this, grant }
				);
		}

		public virtual Task<PersistedGrant> GetAsync( string key )
		{
			if( string.IsNullOrWhiteSpace( key ) ) throw new ArgumentException( "Value cannot be null or whitespace.", nameof(key) );
			
			return _applicationData.ExecuteAsync(
					( dataSession, context ) =>
					{
						return dataSession.GetPersistedGrant( context.key );
					},
					new { @this = this, key }
				);
		}

		public virtual Task<IEnumerable<PersistedGrant>> GetAllAsync( PersistedGrantFilter filter )
		{
			filter.Validate();
			
			return _applicationData.ExecuteAsync(
					( dataSession, context ) =>
					{
						return dataSession.GetPersistedGrants( context.filter.Type, context.filter.SubjectId, context.filter.ClientId, context.filter.SessionId );
					},
					new { @this = this, filter }
				);
		}

		public virtual Task RemoveAsync( string key )
		{
			return _applicationData.PerformAsync(
					( dataSession, context ) =>
					{
						return dataSession.DeletePersistedGrant( context.key );
					},
					new { @this = this, key }
				);
		}

		public virtual Task RemoveAllAsync( PersistedGrantFilter filter )
		{
			filter.Validate();
			
			return _applicationData.PerformAsync(
					( dataSession, context ) =>
					{
						return dataSession.RemovePersistedGrants( context.filter.Type, context.filter.SubjectId, context.filter.ClientId, context.filter.SessionId );
					},
					new { @this = this, filter }
				);
		}
	}
}