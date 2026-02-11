# Quick Reference - Profile Image URLs

## ?? What You Asked For
? **Image paths now return full URLs** like `https://localhost:7001/Images/Patients/image.jpg`  
? **Images are directly accessible** via browser or HTTP client  
? **No additional path construction needed** on frontend  

---

## ?? API Endpoints

### Patient Image Upload
```http
POST /api/Patient/update-profile-image/{userId}
Content-Type: multipart/form-data
Authorization: Bearer {token}

File: [image file]
FileDescription: "My profile picture"
```

**Response:**
```json
{
  "message": "Profile image updated successfully.",
  "data": {
    "filePath": "https://localhost:7001/Images/Patients/guid.jpg"
  }
}
```

### Doctor Image Upload
```http
POST /api/Doctor/update-profile-image/{userId}
Content-Type: multipart/form-data
Authorization: Bearer {token}

File: [image file]
FileDescription: "Professional photo"
```

**Response:**
```json
{
  "message": "Profile image updated successfully.",
  "data": {
    "filePath": "https://localhost:7001/Images/Doctors/guid.jpg"
  }
}
```

---

## ?? Accessing Images

### Copy the URL from response:
```
https://localhost:7001/Images/Patients/a1b2c3d4-e5f6-7890-abcd-ef1234567890.jpg
```

### Paste in browser or use in code:
```html
<!-- HTML -->
<img src="https://localhost:7001/Images/Patients/guid.jpg" />

<!-- Angular -->
<img [src]="patient.profileImage" />

<!-- React -->
<img src={patient.profileImage} />
```

---

## ?? File Storage Location

Images are stored in:
```
HealthConnect/wwwroot/Images/
??? Patients/
??? Doctors/
```

---

## ?? Key Points

1. **Full URL Format:** `{scheme}://{host}/Images/{folder}/{filename}`
2. **Auto-detects:** Scheme (http/https), host, and port from request
3. **Public Access:** Images accessible without authentication once uploaded
4. **Production Ready:** URLs automatically adapt to deployment environment

---

## ? Test It

1. **Upload an image** using Postman or your frontend
2. **Copy the `filePath`** from the response
3. **Paste in browser** - image should load immediately!

---

## ?? Important Notes

- ? URLs work across all environments (dev, staging, production)
- ? Database stores full URLs, not relative paths
- ? Old images automatically deleted on update
- ?? Images are **publicly accessible** (no auth required to view)

---

## ?? Quick Troubleshooting

**Issue:** Image URL returns 404  
**Fix:** Check that file exists in `wwwroot/Images/` folder

**Issue:** URL shows "localhost" in production  
**Fix:** Check that app is using correct Host header from load balancer

**Issue:** CORS error when accessing image  
**Fix:** Already configured - CORS allows Angular app access

---

**?? Everything is set up and ready to use!**
