# CarPark API 🚗

A clean-architecture **.NET 9 Web API** for managing a car park system with **PostgreSQL**.  
Implements vehicle parking allocation, status monitoring, and exit with charge calculation.  

---

## 📂 Project Structure

```
CarParkSolution/
 ├─ CarPark.Api/            → API project (controllers, startup)
 ├─ CarPark.Application/    → DTOs, interfaces
 ├─ CarPark.Domain/         → Entities, enums
 ├─ CarPark.Infrastructure/ → EF Core DbContext, service implementations
 ├─ CarPark.Tests/          → Unit tests
 ├─ Postman/                → Postman collection (CarPark.postman_collection.json)
 └─ README.md               → This file
```

---

## ⚙️ Setup Instructions

### 1. Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for PostgreSQL)

Verify installation:
```bash
dotnet --version   # should show 9.x
docker --version   # should show Docker version
```

---

### 2. Run PostgreSQL with Docker
From solution root:

```bash
docker compose up -d
```

The included `docker-compose.yml` creates:

- **Database**: `carparkdb`  
- **User**: `carpark_user`  
- **Password**: `ChangeThisPassword!`  
- **Port**: `5432`  

---

### 3. Apply EF Migrations & Seed Data
From solution root:

```bash
dotnet ef migrations add InitialCreate   --project CarPark.Infrastructure/CarPark.Infrastructure.csproj   --startup-project CarPark.Api/CarPark.Api.csproj

dotnet ef database update   --project CarPark.Infrastructure/CarPark.Infrastructure.csproj   --startup-project CarPark.Api/CarPark.Api.csproj
```

This will:
- Create the schema  
- Seed **20 parking spaces**  

---

### 4. Run the API
```bash
cd CarPark.Api
dotnet run
```

Swagger UI available at:  
👉 [http://localhost:5269/swagger](http://localhost:5269/swagger)

---

## 📡 API Endpoints

### 1. Park a Vehicle  
**POST** `/parking`

**Request**
```json
{
  "VehicleReg": "ABC123",
  "VehicleType": "Small"
}
```

**Response**
```json
{
  "VehicleReg": "ABC123",
  "SpaceNumber": 1,
  "TimeIn": "2025-09-18T10:00:00Z"
}
```

---

### 2. Get Parking Status  
**GET** `/parking`

**Response**
```json
{
  "AvailableSpaces": 19,
  "OccupiedSpaces": 1
}
```

---

### 3. Exit Vehicle  
**POST** `/parking/exit`

**Request**
```json
{
  "VehicleReg": "ABC123"
}
```

**Response**
```json
{
  "VehicleReg": "ABC123",
  "VehicleCharge": 4.0,
  "TimeIn": "2025-09-18T10:00:00Z",
  "TimeOut": "2025-09-18T10:10:00Z"
}
```

---

## 🧪 Postman Collection & Tests

A ready-to-use **Postman Collection** is included in:

```
CarParkSolution/Postman/CarParkApi.postman_collection.json
```

### Import & Setup
1. Import the collection into Postman  
2. Set environment variable:
   ```json
   {
     "baseUrl": "http://localhost:5269"
   }
   ```

### Built-in Tests

- **POST /parking**
  ```javascript
  pm.test("Status is 200", () => pm.response.to.have.status(200));
  const body = pm.response.json();
  pm.test("Has VehicleReg", () => pm.expect(body.VehicleReg).to.be.a('string'));
  pm.test("Has SpaceNumber", () => pm.expect(body.SpaceNumber).to.be.a('number'));
  pm.test("Has TimeIn", () => pm.expect(body.TimeIn).to.match(/\d{4}-\d{2}-\d{2}T/));
  ```

- **GET /parking**
  ```javascript
  pm.test("Status is 200", () => pm.response.to.have.status(200));
  const body = pm.response.json();
  pm.test("AvailableSpaces", () => pm.expect(body.AvailableSpaces).to.be.a('number'));
  pm.test("OccupiedSpaces", () => pm.expect(body.OccupiedSpaces).to.be.a('number'));
  ```

- **POST /parking/exit**
  ```javascript
  pm.test("Status is 200", () => pm.response.to.have.status(200));
  const body = pm.response.json();
  pm.test("VehicleReg", () => pm.expect(body.VehicleReg).to.be.a('string'));
  pm.test("VehicleCharge >= 0", () => pm.expect(body.VehicleCharge).to.be.at.least(0));
  pm.test("TimeOut > TimeIn", () => {
      const tin = new Date(body.TimeIn).getTime();
      const tout = new Date(body.TimeOut).getTime();
      pm.expect(tout).to.be.greaterThan(tin);
  });
  ```

---

## ✅ Assumptions

- **Charge Calculation**  
  - Per-minute rates: Small (£0.10), Medium (£0.20), Large (£0.40)  
  - +£1 for every **5 full minutes**  
  - Rounded to 2 decimal places  

- **Parking Spaces**: fixed at 20 (seeded)  
- **Time**: stored in UTC  
- **VehicleType**: must be `Small`, `Medium`, or `Large`  

---

## ❓ Queries

1. Should the £1 add-on apply per **full 5 minutes** or **every started 5 minutes**?  
2. Should number of spaces be configurable per environment?  
3. Should historical parking records be exposed via API?  
4. Do we need support for multiple car parks (multi-site)?  

---

## 🧑‍💻 Running Unit Tests

```bash
dotnet test
```

Unit tests validate:
- Allocation of first available space  
- Prevention of duplicate parking  
- Correct charge calculation  

---

## 📥 Clone the Repository with SSH

To get the project locally using SSH:

1. Make sure you have an SSH key added to your GitHub account.  
   - [GitHub guide: Adding a new SSH key](https://docs.github.com/en/authentication/connecting-to-github-with-ssh/adding-a-new-ssh-key-to-your-github-account)

2. Copy the SSH link from the repository page on GitHub.  
   - It looks like this:  
     ```
     git@github.com:<username>/CarParkSolution.git
     ```

3. Clone the repo using:
   ```bash
   git clone git@github.com:<username>/CarParkSolution.git

4. Navigate into the project folder:
  ```bash 
  cd carparksolution
  ```

---

## 📌 Next Steps

- Add authentication (JWT)  
- Extend reporting endpoints  
- Containerize API with Dockerfile  
- Add GitHub Actions for CI/CD  

---
