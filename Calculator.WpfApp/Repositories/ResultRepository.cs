using Calculator.WpfApp.Models;
using Calculator.WpfApp.Models.Domains;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Calculator.WpfApp.Repositories;

internal class ResultRepository
{
	public static async Task<List<Result>> GetResultsAsync()
	{
		using (AppDbContext context = new())
		{
			return await context.Results.ToListAsync();
		}
	}

	public static async Task AddResultAsync(Result result)
	{
		using (AppDbContext context = new())
		{
			context.Results.Add(result);
			await context.SaveChangesAsync();
		}
	}

	public static async Task DeleteResultAsync(int resultId)
	{
		using (AppDbContext context = new())
		{
			var resultToDelete = await context.Results.FindAsync(resultId);
			context.Remove(resultToDelete);
			await context.SaveChangesAsync();
		}
	}

	public static async Task<bool> CheckConnectionAsync()
	{
		try
		{
			using (AppDbContext context = new())
			{
				await context.Database.OpenConnectionAsync();
				await context.Database.CloseConnectionAsync();
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}
}