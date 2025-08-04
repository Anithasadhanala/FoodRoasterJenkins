
using FoodRoasterServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using FoodRoasterServer.Repositories;
using System;

namespace FoodRoasterServer.Repositories
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private readonly AppDbContext _context;
		private readonly DbSet<T> _table;


		public GenericRepository(AppDbContext context)
		{
			_context = context;
			_table = _context.Set<T>(); // _context.Table
		}


		public async Task<T> AddRecord(T entity)
		{
			await _table.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity;
		}
		
		public async Task<T?> GetSingleRecord(int id)
		{
			return await _table.FindAsync(id);
		}

		public async Task<List<T>> GetAllRecords()
		{
			return await _table.ToListAsync();
		}

		public async Task<T> UpdateRecord(T entity)
		{

			_table.Update(entity);
			await _context.SaveChangesAsync();
			_context.ChangeTracker.Clear();
			return entity;
		}


		public async Task<int> DeleteRecord(int id)
		{
			try
			{
				var record = await _table.FindAsync(id);
				if (record != null)
				{
					_table.Remove(record);
					await _context.SaveChangesAsync();
					return 1; // record found
                }
				return 0; // record not found
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
	}
}