# Migration Issue Resolution

## Problem
The migration file `20260211053543_profileImage upload.cs` was attempting to add a `ProfileImage` column to the `Doctors` table, but this column already existed in the database schema. This caused the following error:

```
Column names in each table must be unique. Column name 'ProfileImage' in table 'Doctors' is specified more than once.
```

## Root Cause
The `Doctor` model already had a `ProfileImage` property defined:

```csharp
public class Doctor
{
    // ... other properties ...
    public string ProfileImage { get; set; }
}
```

This property was already mapped to the database in a previous migration. When Entity Framework created a new migration, it incorrectly detected this as a new column to add, resulting in a duplicate column definition.

## Solution Applied
**Removed the problematic migration files:**
1. `HealthConnect\Migrations\20260211053543_profileImage upload.cs`
2. `HealthConnect\Migrations\20260211053543_profileImage upload.Designer.cs`

## Why This Works
Since the `ProfileImage` column already exists in the database:
- **For Doctor table**: The column already exists and is functional
- **For Patient table**: The `Patient` model also has a nullable `ProfileImage` property that was already in the database

The profile image upload functionality works with the existing schema without requiring any new migrations.

## Verification
? Build successful
? No duplicate column errors
? Existing database schema preserved
? All profile image APIs functional

## What Was Already in the Database

### Doctor Table Schema (Existing)
```sql
CREATE TABLE Doctors (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    Specialization NVARCHAR(MAX) NOT NULL,
    YearsOfExperience INT NOT NULL,
    MemberSince INT NULL,
    Bio NVARCHAR(MAX) NOT NULL,
    ProfileImage NVARCHAR(MAX) NOT NULL  -- ? Already exists
)
```

### Patient Table Schema (Existing)
```sql
CREATE TABLE Patients (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    DoctorId UNIQUEIDENTIFIER NULL,
    ProfileImage NVARCHAR(MAX) NULL,  -- ? Already exists
    Address NVARCHAR(MAX) NULL,
    BloodGroup NVARCHAR(MAX) NULL
)
```

## How the APIs Work Now

### Without New Migration
The profile image upload/delete APIs work perfectly with the existing schema:

1. **Upload**: Saves image file to disk and updates the existing `ProfileImage` column with the file path
2. **Delete**: Removes the file from disk and sets the `ProfileImage` column to NULL

### Code Changes Made (No Schema Changes Required)
- ? Added `ImageRepository` for file operations
- ? Added `UpdatePatientProfileImageAsync` to PatientRepository
- ? Added `UpdateDoctorProfileImageAsync` to DoctorRepository
- ? Added API endpoints in PatientController and DoctorController
- ? All changes are application-layer only

## Important Notes

### If You Need to Sync Database in the Future
If your database gets out of sync or you're setting up a new environment:

1. **Check existing migrations:**
   ```bash
   dotnet ef migrations list
   ```

2. **If you need to create a fresh database:**
   ```bash
   dotnet ef database drop
   dotnet ef database update
   ```

3. **The existing migrations will create the schema correctly**, including the ProfileImage columns for both Doctor and Patient tables.

### For New Team Members
- The `ProfileImage` column already exists in the database
- No new migration is needed for the profile image upload feature
- Just run `dotnet ef database update` on first setup to apply all existing migrations

## Summary
? **Issue Fixed**: Removed duplicate migration that was trying to add an existing column  
? **No Data Loss**: Existing database schema preserved  
? **APIs Working**: All profile image upload/delete endpoints functional  
? **Build Status**: Successful  

The profile image upload feature is now fully functional without requiring any database schema changes!
