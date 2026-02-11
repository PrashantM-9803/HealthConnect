# Profile Image Upload API Documentation

## Overview
This document provides detailed information about the profile image upload APIs for both Patient and Doctor entities in the HealthConnect application.

## Features Implemented

### 1. Image Repository Service
- **File**: `HealthConnect\Repositories\ImageRepository.cs`
- **Interface**: `HealthConnect\Repositories\IImageRepository.cs`
- **Functionality**:
  - Upload images with validation
  - Delete existing images
  - Support for multiple image formats (jpg, jpeg, png, gif, bmp)
  - Maximum file size: 5MB
  - Automatic file naming using GUID to prevent conflicts
  - Organized storage in separate folders (Patients/Doctors)

### 2. Data Transfer Objects (DTOs)
- **ImageUploadDto**: Used for receiving image upload requests
  - `IFormFile File` (Required)
  - `string? FileDescription` (Optional)

- **ImageUploadResponseDto**: Used for sending upload response
  - `string FileName`
  - `string FilePath`
  - `string? FileDescription`
  - `string FileExtension`
  - `long FileSizeInBytes`
  - `DateTime UploadedAt`

---

## Patient Profile Image APIs

### 1. Update Patient Profile Image
**Endpoint**: `POST /api/Patient/update-profile-image/{userId}`

**Authorization**: Required (Roles: PATIENT, ADMIN)

**Parameters**:
- `userId` (path parameter): GUID of the user

**Request Body** (multipart/form-data):
```
File: [image file]
FileDescription: [optional description]
```

**Success Response** (200 OK):
```json
{
  "message": "Profile image updated successfully.",
  "data": {
    "fileName": "example.jpg",
    "filePath": "https://localhost:7001/Images/Patients/a1b2c3d4-e5f6-7890-abcd-ef1234567890.jpg",
    "fileDescription": "Profile picture",
    "fileExtension": ".jpg",
    "fileSizeInBytes": 245678,
    "uploadedAt": "2024-01-15T10:30:00Z"
  }
}
```

**Error Responses**:
- **404 Not Found**: Patient not found
- **400 Bad Request**: Invalid image file
- **500 Internal Server Error**: Upload failure

**Example cURL Request**:
```bash
curl -X POST "https://localhost:5001/api/Patient/update-profile-image/123e4567-e89b-12d3-a456-426614174000" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -F "File=@/path/to/image.jpg" \
  -F "FileDescription=My profile picture"
```

**Example C# HttpClient Request**:
```csharp
using var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

using var content = new MultipartFormDataContent();
var fileContent = new ByteArrayContent(File.ReadAllBytes("image.jpg"));
fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
content.Add(fileContent, "File", "image.jpg");
content.Add(new StringContent("My profile picture"), "FileDescription");

var response = await client.PostAsync(
    $"https://localhost:5001/api/Patient/update-profile-image/{userId}", 
    content
);
```

---

### 2. Delete Patient Profile Image
**Endpoint**: `DELETE /api/Patient/delete-profile-image/{userId}`

**Authorization**: Required (Roles: PATIENT, ADMIN)

**Parameters**:
- `userId` (path parameter): GUID of the user

**Success Response** (200 OK):
```json
{
  "message": "Profile image deleted successfully."
}
```

**Error Responses**:
- **404 Not Found**: Patient not found
- **400 Bad Request**: No profile image to delete
- **500 Internal Server Error**: Deletion failure

**Example cURL Request**:
```bash
curl -X DELETE "https://localhost:5001/api/Patient/delete-profile-image/123e4567-e89b-12d3-a456-426614174000" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

## Doctor Profile Image APIs

### 3. Update Doctor Profile Image
**Endpoint**: `POST /api/Doctor/update-profile-image/{userId}`

**Authorization**: Required (Roles: DOCTOR, ADMIN)

**Parameters**:
- `userId` (path parameter): GUID of the user

**Request Body** (multipart/form-data):
```
File: [image file]
FileDescription: [optional description]
```

**Success Response** (200 OK):
```json
{
  "message": "Profile image updated successfully.",
  "data": {
    "fileName": "doctor_photo.png",
    "filePath": "https://localhost:7001/Images/Doctors/b2c3d4e5-f6a7-8901-bcde-f12345678901.png",
    "fileDescription": "Professional headshot",
    "fileExtension": ".png",
    "fileSizeInBytes": 312456,
    "uploadedAt": "2024-01-15T11:45:00Z"
  }
}
```

**Error Responses**:
- **404 Not Found**: Doctor not found
- **400 Bad Request**: Invalid image file
- **500 Internal Server Error**: Upload failure

**Example JavaScript Fetch Request**:
```javascript
const formData = new FormData();
formData.append('File', fileInput.files[0]);
formData.append('FileDescription', 'Professional headshot');

const response = await fetch(`/api/Doctor/update-profile-image/${userId}`, {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`
  },
  body: formData
});

const result = await response.json();
console.log(result);
```

---

### 4. Delete Doctor Profile Image
**Endpoint**: `DELETE /api/Doctor/delete-profile-image/{userId}`

**Authorization**: Required (Roles: DOCTOR, ADMIN)

**Parameters**:
- `userId` (path parameter): GUID of the user

**Success Response** (200 OK):
```json
{
  "message": "Profile image deleted successfully."
}
```

**Error Responses**:
- **404 Not Found**: Doctor not found
- **400 Bad Request**: No profile image to delete
- **500 Internal Server Error**: Deletion failure

---

## Image Validation Rules

### Allowed File Extensions
- `.jpg`
- `.jpeg`
- `.png`
- `.gif`
- `.bmp`

### File Size Limit
- Maximum: 5MB (5,242,880 bytes)

### Validation Error Message
```json
{
  "message": "Invalid image file. Allowed formats: jpg, jpeg, png, gif, bmp. Max size: 5MB."
}
```

---

## File Storage Structure

Images are stored in the `wwwroot` folder and served as static files:

```
HealthConnect/
??? wwwroot/
    ??? Images/
        ??? Patients/
        ?   ??? {guid1}.jpg
        ?   ??? {guid2}.png
        ?   ??? ...
        ??? Doctors/
            ??? {guid1}.jpg
            ??? {guid2}.png
            ??? ...
```

Each uploaded file is assigned a unique GUID-based filename to prevent naming conflicts and ensure uniqueness.

### Accessing Images
Images can be accessed directly via HTTP using the full URL stored in the database:
- **Format**: `https://localhost:{port}/Images/{Patients|Doctors}/{filename}`
- **Example**: `https://localhost:7001/Images/Patients/a1b2c3d4-e5f6-7890-abcd-ef1234567890.jpg`

The images are served as static files through ASP.NET Core's Static Files Middleware.

---

## Implementation Details

### Key Components

1. **ImageRepository** (`IImageRepository`)
   - Handles all file operations
   - Validates image format and size
   - Manages file upload and deletion
   - Creates directory structure automatically
   - **Generates full URLs** using `IHttpContextAccessor` to build absolute URLs based on current request

2. **PatientRepository** (`IPatientRepository`)
   - Added `UpdatePatientProfileImageAsync` method
   - Updates patient's ProfileImage property with full URL

3. **DoctorRepository** (`IDoctorRepository`)
   - Added `UpdateDoctorProfileImageAsync` method
   - Updates doctor's ProfileImage property with full URL

4. **Controllers**
   - `PatientController`: Added 2 new endpoints for image management
   - `DoctorController`: Added 2 new endpoints for image management

5. **Static Files Middleware**
   - Configured in `Program.cs` with `app.UseStaticFiles()`
   - Serves images from `wwwroot/Images` folder
   - Images accessible via `https://localhost:port/Images/...`

### Design Patterns Used

1. **Repository Pattern**: Separation of data access logic
2. **Dependency Injection**: Loose coupling between components
3. **DTO Pattern**: Clean separation between domain models and API contracts
4. **Single Responsibility Principle**: Each class has one specific purpose
5. **Async/Await Pattern**: Non-blocking I/O operations

### Security Considerations

1. **Authentication & Authorization**: All endpoints require JWT authentication
2. **Role-Based Access Control**: 
   - Patients can only update their own images
   - Doctors can only update their own images
   - Admins can update any image
3. **File Validation**: Strict validation on file type and size
4. **Unique Filenames**: GUID-based naming prevents overwriting
5. **Path Security**: Files stored outside web root with controlled access

---

## Testing with Postman

### Setup
1. Obtain JWT token by logging in
2. Set Authorization header: `Bearer {token}`
3. Set request type to POST or DELETE

### For Upload (POST)
1. Select Body tab
2. Choose "form-data"
3. Add key "File" with type "File" and select your image
4. Add key "FileDescription" with type "Text" (optional)
5. Send request

### For Delete (DELETE)
1. No body required
2. Just send the DELETE request with proper authorization

---

## Error Handling

All endpoints include comprehensive error handling:

- **404 Not Found**: User/Patient/Doctor doesn't exist
- **400 Bad Request**: Invalid input or validation failure
- **401 Unauthorized**: Missing or invalid JWT token
- **403 Forbidden**: Insufficient permissions
- **500 Internal Server Error**: Unexpected server errors

---

## Best Practices Followed

1. ? Async/await for all I/O operations
2. ? Proper exception handling with try-catch blocks
3. ? Input validation before processing
4. ? Cleanup of old images before uploading new ones
5. ? Meaningful HTTP status codes and error messages
6. ? Separation of concerns (Repository, Service, Controller)
7. ? Dependency injection for testability
8. ? Consistent naming conventions
9. ? XML documentation comments
10. ? Authorization on all endpoints

---

## Future Enhancements

Potential improvements for consideration:

1. **Image Resizing**: Automatically resize images to standard dimensions
2. **Thumbnail Generation**: Create thumbnail versions for list views
3. **Cloud Storage**: Integrate with Azure Blob Storage or AWS S3
4. **Image Optimization**: Compress images to reduce storage
5. **CDN Integration**: Serve images through a CDN for better performance
6. **Image Versioning**: Keep history of profile images
7. **Batch Upload**: Support multiple image uploads
8. **Image Cropping**: Allow users to crop images before upload

---

## Troubleshooting

### Common Issues

1. **"Invalid image file" error**
   - Verify file extension is supported
   - Check file size is under 5MB
   - Ensure file is not corrupted

2. **401 Unauthorized**
   - Verify JWT token is valid and not expired
   - Check Authorization header format

3. **403 Forbidden**
   - Verify user has correct role (PATIENT/DOCTOR/ADMIN)
   - Check if user is trying to update their own profile

4. **500 Internal Server Error**
   - Check server logs for detailed error
   - Verify Images directory has write permissions
   - Ensure database connection is working

---

## Support

For issues or questions regarding these APIs, please contact the development team or refer to the main application documentation.
