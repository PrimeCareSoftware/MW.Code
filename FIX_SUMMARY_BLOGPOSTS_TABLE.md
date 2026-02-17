# BlogPosts Table Migration Fix

## Issue
The API was failing with a PostgreSQL error when accessing the `/api/blog-posts/admin/all` endpoint:

```
Npgsql.PostgresException (0x80004005): 42P01: relation "BlogPosts" does not exist
```

## Root Cause
A migration file was manually created (`20260217200000_AddBlogPostsTable.cs`) without using Entity Framework Core's migration tooling. This resulted in:
- The migration file existing but not being properly integrated
- The model snapshot (`MedicSoftDbContextModelSnapshot.cs`) not being updated
- The database schema not including the BlogPosts table

## Solution
1. **Removed** the manually created migration file
2. **Generated** a proper migration using Entity Framework Core tools:
   ```bash
   dotnet ef migrations add AddBlogPostsTable --context MedicSoftDbContext --startup-project ../MedicSoft.Api --output-dir Migrations/PostgreSQL
   ```
3. **Verified** the migration includes:
   - Proper table creation with all columns
   - Indexes on Category, IsPublished, PublishedAt
   - Unique constraint on Slug
   - Model snapshot update

## Migration Details

### Table: BlogPosts
- **Primary Key**: Id (uuid)
- **Required Columns**:
  - Title (varchar 500)
  - Slug (varchar 600) - unique index
  - Content (text)
  - Excerpt (varchar 1000)
  - Category (varchar 100)
  - AuthorName (varchar 200)
  - AuthorId (uuid)
  - TenantId (varchar 100)
  - CreatedAt (timestamp)
  - IsPublished (boolean)
  - ReadTimeMinutes (integer)

- **Optional Columns**:
  - FeaturedImage (varchar 500)
  - PublishedAt (timestamp)
  - UpdatedAt (timestamp)

### Indexes
- `IX_BlogPosts_Category` - for filtering by category
- `IX_BlogPosts_IsPublished` - for filtering published posts
- `IX_BlogPosts_PublishedAt` - for ordering by publish date
- `IX_BlogPosts_Slug` - unique constraint for URL slugs

## How to Apply the Migration

### Option 1: Using the Migration Script
```bash
./run-all-migrations.sh
```

### Option 2: Using dotnet ef directly
```bash
cd src/MedicSoft.Api
dotnet ef database update --context MedicSoftDbContext
```

### Option 3: Automatic on Application Start
Set in `appsettings.json`:
```json
{
  "Database": {
    "ApplyMigrations": true
  }
}
```

## Verification
After applying the migration, the following endpoints will work:
- `GET /api/blog-posts/published` - Public access to published posts
- `GET /api/blog-posts/admin/all` - Admin access to all posts (requires authentication)
- `GET /api/blog-posts/{slug}` - Get post by slug
- `POST /api/blog-posts` - Create new post (admin only)
- `PUT /api/blog-posts/{id}` - Update post (admin only)
- `DELETE /api/blog-posts/{id}` - Delete post (admin only)

## Files Changed
1. **Removed**: `src/MedicSoft.Repository/Migrations/PostgreSQL/20260217200000_AddBlogPostsTable.cs`
2. **Added**: 
   - `src/MedicSoft.Repository/Migrations/PostgreSQL/20260217212502_AddBlogPostsTable.cs`
   - `src/MedicSoft.Repository/Migrations/PostgreSQL/20260217212502_AddBlogPostsTable.Designer.cs`
3. **Updated**: `src/MedicSoft.Repository/Migrations/PostgreSQL/MedicSoftDbContextModelSnapshot.cs`

## Security Review
✅ **SQL Injection**: Protected by EF Core parameterized queries  
✅ **Access Control**: Proper [Authorize] and IsSystemOwner() checks  
✅ **Input Validation**: Page and pageSize parameters validated  
✅ **Data Exposure**: No sensitive data in migration  

## Related Files
- Entity: `src/MedicSoft.Domain/Entities/BlogPost.cs`
- Configuration: `src/MedicSoft.Repository/Configurations/BlogPostConfiguration.cs`
- Repository: `src/MedicSoft.Repository/Repositories/BlogPostRepository.cs`
- Controller: `src/MedicSoft.Api/Controllers/BlogPostsController.cs`
- DbContext: `src/MedicSoft.Repository/Context/MedicSoftDbContext.cs`
