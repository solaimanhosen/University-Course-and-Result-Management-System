using Microsoft.EntityFrameworkCore;

namespace University.Api.Data;

internal sealed class UniversityDbContext(DbContextOptions<UniversityDbContext> options)
    : DbContext(options);
