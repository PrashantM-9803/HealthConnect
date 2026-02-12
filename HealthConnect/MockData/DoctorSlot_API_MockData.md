# Doctor Slot API Mock Data

This document contains mock data for all DoctorSlot API endpoints.

---

## 1. POST `/api/DoctorSlot/generate` - Generate Time Slots

### Description
Generate time slots for a doctor for a date range.

### Authorization
Required - Roles: DOCTOR, ADMIN

### Request Body
```json
{
  "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "startDate": "2024-03-01T00:00:00",
  "endDate": "2024-03-07T00:00:00"
}
```

### Response (200 OK)
```json
{
  "message": "Successfully generated 42 slots for doctor.",
  "totalGenerated": 42,
  "slots": [
    {
      "id": "a1b2c3d4-1111-2222-3333-444444444444",
      "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2024-03-01T00:00:00",
      "startTime": "09:00:00",
      "endTime": "09:30:00",
      "isBooked": false
    },
    {
      "id": "a1b2c3d4-1111-2222-3333-444444444445",
      "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2024-03-01T00:00:00",
      "startTime": "09:30:00",
      "endTime": "10:00:00",
      "isBooked": false
    },
    {
      "id": "a1b2c3d4-1111-2222-3333-444444444446",
      "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2024-03-01T00:00:00",
      "startTime": "10:00:00",
      "endTime": "10:30:00",
      "isBooked": false
    },
    {
      "id": "a1b2c3d4-1111-2222-3333-444444444447",
      "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2024-03-01T00:00:00",
      "startTime": "10:30:00",
      "endTime": "11:00:00",
      "isBooked": false
    }
  ]
}
```

### Error Response - Invalid Date Range (400 Bad Request)
```json
{
  "message": "Start date must be before or equal to end date."
}
```

### Error Response - Doctor Not Found (404 Not Found)
```json
{
  "message": "Doctor not found."
}
```

---

## 2. GET `/api/DoctorSlot/doctor/{doctorId}/available` - Get Available Slots

### Description
Get available (unbooked) slots for a doctor, optionally filtered by date.

### Authorization
Not required

### Request Examples

#### Example 1: Get all available slots for a doctor
```
GET /api/DoctorSlot/doctor/3fa85f64-5717-4562-b3fc-2c963f66afa6/available
```

#### Example 2: Get available slots for a specific date
```
GET /api/DoctorSlot/doctor/3fa85f64-5717-4562-b3fc-2c963f66afa6/available?date=2024-03-15
```

### Response (200 OK)
```json
{
  "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "doctorName": "Dr. Sarah Johnson",
  "totalAvailableSlots": 8,
  "slots": [
    {
      "slotId": "b1c2d3e4-5555-6666-7777-888888888888",
      "date": "2024-03-15T00:00:00",
      "startTime": "09:00:00",
      "endTime": "09:30:00",
      "timeDisplay": "09:00 - 09:30"
    },
    {
      "slotId": "b1c2d3e4-5555-6666-7777-888888888889",
      "date": "2024-03-15T00:00:00",
      "startTime": "09:30:00",
      "endTime": "10:00:00",
      "timeDisplay": "09:30 - 10:00"
    },
    {
      "slotId": "b1c2d3e4-5555-6666-7777-888888888890",
      "date": "2024-03-15T00:00:00",
      "startTime": "10:00:00",
      "endTime": "10:30:00",
      "timeDisplay": "10:00 - 10:30"
    },
    {
      "slotId": "b1c2d3e4-5555-6666-7777-888888888891",
      "date": "2024-03-15T00:00:00",
      "startTime": "11:00:00",
      "endTime": "11:30:00",
      "timeDisplay": "11:00 - 11:30"
    },
    {
      "slotId": "b1c2d3e4-5555-6666-7777-888888888892",
      "date": "2024-03-15T00:00:00",
      "startTime": "14:00:00",
      "endTime": "14:30:00",
      "timeDisplay": "14:00 - 14:30"
    },
    {
      "slotId": "b1c2d3e4-5555-6666-7777-888888888893",
      "date": "2024-03-15T00:00:00",
      "startTime": "14:30:00",
      "endTime": "15:00:00",
      "timeDisplay": "14:30 - 15:00"
    },
    {
      "slotId": "b1c2d3e4-5555-6666-7777-888888888894",
      "date": "2024-03-15T00:00:00",
      "startTime": "15:00:00",
      "endTime": "15:30:00",
      "timeDisplay": "15:00 - 15:30"
    },
    {
      "slotId": "b1c2d3e4-5555-6666-7777-888888888895",
      "date": "2024-03-15T00:00:00",
      "startTime": "16:00:00",
      "endTime": "16:30:00",
      "timeDisplay": "16:00 - 16:30"
    }
  ]
}
```

### Error Response - Doctor Not Found (404 Not Found)
```json
{
  "message": "Doctor not found."
}
```

---

## 3. GET `/api/DoctorSlot/doctor/{doctorId}` - Get All Slots

### Description
Get all slots (both available and booked) for a doctor, optionally filtered by date range.

### Authorization
Required - Roles: DOCTOR, ADMIN

### Request Examples

#### Example 1: Get all slots without date filter
```
GET /api/DoctorSlot/doctor/3fa85f64-5717-4562-b3fc-2c963f66afa6
```

#### Example 2: Get slots for a date range
```
GET /api/DoctorSlot/doctor/3fa85f64-5717-4562-b3fc-2c963f66afa6?startDate=2024-03-01&endDate=2024-03-31
```

### Response (200 OK)
```json
{
  "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "doctorName": "Dr. Sarah Johnson",
  "totalSlots": 12,
  "bookedSlots": 4,
  "availableSlots": 8,
  "slots": [
    {
      "id": "c1d2e3f4-9999-aaaa-bbbb-cccccccccccc",
      "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2024-03-20T00:00:00",
      "startTime": "09:00:00",
      "endTime": "09:30:00",
      "isBooked": true
    },
    {
      "id": "c1d2e3f4-9999-aaaa-bbbb-cccccccccccd",
      "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2024-03-20T00:00:00",
      "startTime": "09:30:00",
      "endTime": "10:00:00",
      "isBooked": false
    },
    {
      "id": "c1d2e3f4-9999-aaaa-bbbb-cccccccccce",
      "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2024-03-20T00:00:00",
      "startTime": "10:00:00",
      "endTime": "10:30:00",
      "isBooked": false
    },
    {
      "id": "c1d2e3f4-9999-aaaa-bbbb-ccccccccccf",
      "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2024-03-20T00:00:00",
      "startTime": "10:30:00",
      "endTime": "11:00:00",
      "isBooked": true
    },
    {
      "id": "c1d2e3f4-9999-aaaa-bbbb-ccccccccc10",
      "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2024-03-20T00:00:00",
      "startTime": "11:00:00",
      "endTime": "11:30:00",
      "isBooked": false
    },
    {
      "id": "c1d2e3f4-9999-aaaa-bbbb-ccccccccc11",
      "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2024-03-20T00:00:00",
      "startTime": "14:00:00",
      "endTime": "14:30:00",
      "isBooked": true
    },
    {
      "id": "c1d2e3f4-9999-aaaa-bbbb-ccccccccc12",
      "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2024-03-20T00:00:00",
      "startTime": "14:30:00",
      "endTime": "15:00:00",
      "isBooked": false
    },
    {
      "id": "c1d2e3f4-9999-aaaa-bbbb-ccccccccc13",
      "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2024-03-20T00:00:00",
      "startTime": "15:00:00",
      "endTime": "15:30:00",
      "isBooked": false
    },
    {
      "id": "c1d2e3f4-9999-aaaa-bbbb-ccccccccc14",
      "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2024-03-20T00:00:00",
      "startTime": "15:30:00",
      "endTime": "16:00:00",
      "isBooked": true
    },
    {
      "id": "c1d2e3f4-9999-aaaa-bbbb-ccccccccc15",
      "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2024-03-20T00:00:00",
      "startTime": "16:00:00",
      "endTime": "16:30:00",
      "isBooked": false
    },
    {
      "id": "c1d2e3f4-9999-aaaa-bbbb-ccccccccc16",
      "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2024-03-20T00:00:00",
      "startTime": "16:30:00",
      "endTime": "17:00:00",
      "isBooked": false
    },
    {
      "id": "c1d2e3f4-9999-aaaa-bbbb-ccccccccc17",
      "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "date": "2024-03-20T00:00:00",
      "startTime": "17:00:00",
      "endTime": "17:30:00",
      "isBooked": false
    }
  ]
}
```

### Error Response - Doctor Not Found (404 Not Found)
```json
{
  "message": "Doctor not found."
}
```

---

## 4. GET `/api/DoctorSlot/{slotId}` - Get Slot By ID

### Description
Get details of a specific slot by its ID.

### Authorization
Not required

### Request Example
```
GET /api/DoctorSlot/d1e2f3a4-bbbb-cccc-dddd-eeeeeeeeeeee
```

### Response (200 OK)
```json
{
  "id": "d1e2f3a4-bbbb-cccc-dddd-eeeeeeeeeeee",
  "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "date": "2024-03-25T00:00:00",
  "startTime": "10:00:00",
  "endTime": "10:30:00",
  "isBooked": false
}
```

### Error Response - Slot Not Found (404 Not Found)
```json
{
  "message": "Slot not found."
}
```

---

## 5. DELETE `/api/DoctorSlot/{slotId}` - Delete Slot

### Description
Delete a specific slot (only if it's not booked).

### Authorization
Required - Roles: DOCTOR, ADMIN

### Request Example
```
DELETE /api/DoctorSlot/d1e2f3a4-bbbb-cccc-dddd-eeeeeeeeeeee
```

### Response (200 OK)
```json
{
  "message": "Slot deleted successfully."
}
```

### Error Response - Slot Not Found or Already Booked (400 Bad Request)
```json
{
  "message": "Slot not found or is already booked and cannot be deleted."
}
```

---

## 6. DELETE `/api/DoctorSlot/doctor/{doctorId}/date-range` - Delete Slots for Date Range

### Description
Delete all unbooked slots for a doctor within a specified date range.

### Authorization
Required - Roles: DOCTOR, ADMIN

### Request Example
```
DELETE /api/DoctorSlot/doctor/3fa85f64-5717-4562-b3fc-2c963f66afa6/date-range?startDate=2024-03-01&endDate=2024-03-07
```

### Response (200 OK)
```json
{
  "message": "Unbooked slots deleted successfully for the specified date range."
}
```

### Error Response - Invalid Date Range (400 Bad Request)
```json
{
  "message": "Start date must be before or equal to end date."
}
```

### Error Response - No Slots Found (404 Not Found)
```json
{
  "message": "No unbooked slots found for the specified date range."
}
```

---

## 7. GET `/api/DoctorSlot/doctor/{doctorId}/available-dates` - Get Available Dates

### Description
Get all dates that have at least one available slot for a doctor, optionally filtered by date range.

### Authorization
Not required

### Request Examples

#### Example 1: Get available dates for next month
```
GET /api/DoctorSlot/doctor/3fa85f64-5717-4562-b3fc-2c963f66afa6/available-dates
```

#### Example 2: Get available dates for specific range
```
GET /api/DoctorSlot/doctor/3fa85f64-5717-4562-b3fc-2c963f66afa6/available-dates?startDate=2024-03-01&endDate=2024-04-30
```

### Response (200 OK)
```json
{
  "doctorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "doctorName": "Dr. Sarah Johnson",
  "totalAvailableDates": 15,
  "dates": [
    "2024-03-15T00:00:00",
    "2024-03-16T00:00:00",
    "2024-03-18T00:00:00",
    "2024-03-19T00:00:00",
    "2024-03-20T00:00:00",
    "2024-03-22T00:00:00",
    "2024-03-23T00:00:00",
    "2024-03-25T00:00:00",
    "2024-03-26T00:00:00",
    "2024-03-27T00:00:00",
    "2024-03-29T00:00:00",
    "2024-03-30T00:00:00",
    "2024-04-01T00:00:00",
    "2024-04-02T00:00:00",
    "2024-04-03T00:00:00"
  ]
}
```

### Error Response - Doctor Not Found (404 Not Found)
```json
{
  "message": "Doctor not found."
}
```

---

## Common Test Data Reference

### Test Doctor IDs
- `3fa85f64-5717-4562-b3fc-2c963f66afa6` - Dr. Sarah Johnson (Cardiologist)
- `4fb85f64-6828-5673-c4gd-3d074g77bgb7` - Dr. Michael Chen (Dermatologist)
- `5gc85f64-7939-6784-d5he-4e185h88chc8` - Dr. Emily Rodriguez (Pediatrician)

### Test Slot IDs
- `a1b2c3d4-1111-2222-3333-444444444444` - Available slot (March 1, 09:00-09:30)
- `b1c2d3e4-5555-6666-7777-888888888888` - Available slot (March 15, 09:00-09:30)
- `c1d2e3f4-9999-aaaa-bbbb-cccccccccccc` - Booked slot (March 20, 09:00-09:30)
- `d1e2f3a4-bbbb-cccc-dddd-eeeeeeeeeeee` - Available slot (March 25, 10:00-10:30)

### Common Date Ranges
- **Current Week**: 2024-03-15 to 2024-03-21
- **Current Month**: 2024-03-01 to 2024-03-31
- **Next Month**: 2024-04-01 to 2024-04-30

### Typical Working Hours
- Morning slots: 09:00 - 12:00
- Afternoon slots: 14:00 - 17:30
- Slot duration: 30 minutes

---

## Postman Collection Tips

### Environment Variables
Create these variables in Postman:
```
base_url: http://localhost:5000 (or your API URL)
doctor_id: 3fa85f64-5717-4562-b3fc-2c963f66afa6
slot_id: d1e2f3a4-bbbb-cccc-dddd-eeeeeeeeeeee
auth_token: (your JWT token after login)
```

### Authorization Header
For endpoints requiring authorization:
```
Authorization: Bearer {{auth_token}}
```

### Common Test Scenarios

1. **Complete Slot Lifecycle**:
   - Generate slots for a week
   - View all slots for the doctor
   - View available slots
   - Book a slot (via appointment API)
   - Delete unused slots

2. **Patient Journey**:
   - Get available dates for doctor
   - Get available slots for specific date
   - Get specific slot details
   - Book the slot

3. **Doctor Management**:
   - Generate slots for multiple weeks
   - View all slots (booked and available)
   - Delete slots for specific date range
   - Regenerate slots

---

## Notes

- All timestamps should use ISO 8601 format
- TimeSpan values are in HH:mm:ss format
- All IDs are GUIDs (UUID v4)
- Dates are stored as UTC but displayed in the local timezone
- Booked slots cannot be deleted
- Past slots are typically filtered out in available slot queries
