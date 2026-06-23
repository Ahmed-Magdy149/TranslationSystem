# Production Reports Module - Complete Implementation

## Overview
Complete production reporting system with 4 comprehensive report types: Service Performance, User Productivity, Late Tasks, and Project Reports.

## Features Implemented

### 1. Service Performance Report
- **Metrics Tracked:**
  - Service type breakdown (Translation, Revision, TEP, MTPE, QA, LQA, LSO, Shooting, Proofreading)
  - Translator productivity (words/hour)
  - Senior Translator productivity (words/hour)
  - Reviewer productivity (words/hour)
  - SPOC tracking
  - Total words processed
  - Calculated hours vs actual hours
  - Performance percentage

### 2. User Productivity Report
- **Metrics Tracked:**
  - User name and role
  - Total tasks assigned
  - Completed tasks count
  - Total words completed
  - Expected hours (based on service rates)
  - Actual completion time
  - On-time tasks count
  - Late tasks count
  - Performance percentage

### 3. Late Tasks Report
- **Information Displayed:**
  - Task title
  - Assigned user
  - Deadline date
  - Actual completion date
  - Delay duration (days/hours)
  - Delay reason/comment

### 4. Project Report
- **Metrics Tracked:**
  - Project name
  - Total tasks in project
  - Completed tasks
  - Pending tasks
  - Total words
  - Performance percentage

## Database Schema Changes

### New Entities

#### ServiceRate
```csharp
- Id (ObjectId)
- ServiceType (string) - Translation, Revision, etc.
- Role (string) - Translator, Senior Translator, Reviewer
- WordsPerHour (int) - Productivity rate
- CreatedAt (DateTime)
- UpdatedAt (DateTime?)
```

#### Enhanced TaskHistory
```csharp
- Id (ObjectId)
- TaskId (string)
- UserId (string)
- Action (string)
- Comment (string)
- OldStatus (TaskStatus?) - NEW
- NewStatus (TaskStatus?) - NEW
- DelayReason (string?) - NEW
- CreatedAt (DateTime)
```

#### Enhanced TranslationTask
```csharp
- ServiceType (string) - NEW
- ProjectId (string?) - NEW
- ProjectName (string?) - NEW
- AssignedTeamLeaderId (string?) - NEW
- AssignedReviewerId (string?) - NEW
- AssignedProjectManagerId (string?) - NEW
- CompletedDate (DateTime?) - NEW
```

## API Endpoints

### Reports Controller (`/api/reports`)

1. **POST /api/reports/service-performance**
   - Body: `ReportFilterDto`
   - Returns: `List<ServicePerformanceDto>`

2. **POST /api/reports/user-productivity**
   - Body: `ReportFilterDto`
   - Returns: `List<UserProductivityDto>`

3. **POST /api/reports/late-tasks**
   - Body: `ReportFilterDto`
   - Returns: `List<LateTaskDto>`

4. **POST /api/reports/project-report**
   - Body: `ReportFilterDto`
   - Returns: `List<ProjectReportDto>`

### Service Rates Controller (`/api/servicerates`)

1. **GET /api/servicerates** - Get all service rates
2. **GET /api/servicerates/{id}** - Get by ID
3. **POST /api/servicerates** - Create new rate
4. **PUT /api/servicerates/{id}** - Update rate
5. **DELETE /api/servicerates/{id}** - Delete rate
6. **GET /api/servicerates/service/{serviceType}/role/{role}** - Get specific rate

## Filter Options

```csharp
public class ReportFilterDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; } = DateTime.UtcNow;
    public string? ProjectId { get; set; }
    public string? UserId { get; set; }
    public string? Role { get; set; }
    public string? ServiceType { get; set; }
    public string? Status { get; set; }
}
```

## UI Components

### Reports Page (`/reports`)
- **Tab-based interface** with 4 tabs
- **Advanced filters:**
  - Date range (From/To)
  - Service type dropdown
  - Status dropdown
  - Project filter
  - User filter
  - Role filter
- **Export to Excel** button (ready for implementation)
- **Responsive tables** with sorting
- **Real-time data loading**

### PageHeader Component
- Reusable header component
- Title and subtitle support
- Optional action buttons

## Calculation Logic

### Performance Percentage
```
Performance % = (Expected Hours / Actual Hours) × 100
```

### Expected Hours
```
Expected Hours = Total Words / Words Per Hour (from ServiceRate)
```

### Delay Duration
```
Delay = Completed Date - Deadline
```

### Task Eligibility for Reports
Tasks are included in reports only when:
1. Status = `Approved` (completed by translator AND approved by reviewer)
2. Within the selected date range
3. Match the applied filters

## Service Rates Configuration

Default rates can be configured via API or database:

| Service | Role | Words/Hour |
|---------|------|------------|
| Translation | Translator | 200 |
| Translation | Senior Translator | 275 |
| Translation | Reviewer | 300 |
| Revision | Translator | 250 |
| TEP | Translator | 180 |
| MTPE | Translator | 350 |

## Files Created/Modified

### Core Layer
- ✅ `TMS.Core/Entities/ServiceRate.cs` - NEW
- ✅ `TMS.Core/Entities/TaskHistory.cs` - MODIFIED (added status tracking)
- ✅ `TMS.Core/Entities/TranslationTask.cs` - MODIFIED (added project & team fields)
- ✅ `TMS.Core/DTOs/ReportDto.cs` - NEW
- ✅ `TMS.Core/Interfaces/IServiceRateRepository.cs` - NEW
- ✅ `TMS.Core/Interfaces/ITaskHistoryRepository.cs` - NEW
- ✅ `TMS.Core/Interfaces/IReportService.cs` - NEW
- ✅ `TMS.Core/Interfaces/IServiceRateService.cs` - NEW

### Application Layer
- ✅ `TMS.Application/Services/ReportService.cs` - NEW
- ✅ `TMS.Application/Services/ServiceRateService.cs` - NEW

### Infrastructure Layer
- ✅ `TMS.Infrastructure/Repositories/ServiceRateRepository.cs` - NEW
- ✅ `TMS.Infrastructure/Repositories/TaskHistoryRepository.cs` - NEW
- ✅ `TMS.Infrastructure/Data/MongoDbContext.cs` - MODIFIED (added ServiceRates collection)

### Web Layer
- ✅ `TMS.Web/Controllers/ReportsController.cs` - NEW
- ✅ `TMS.Web/Controllers/ServiceRatesController.cs` - NEW
- ✅ `TMS.Web/Components/Pages/Reports.razor` - NEW
- ✅ `TMS.Web/Components/Pages/Reports.razor.cs` - NEW
- ✅ `TMS.Web/Components/Pages/Reports.razor.css` - NEW
- ✅ `TMS.Web/Components/Shared/PageHeader.razor` - NEW
- ✅ `TMS.Web/Components/Shared/PageHeader.razor.css` - NEW
- ✅ `TMS.Web/Program.cs` - MODIFIED (registered new services)

## Build Status
✅ **Build Succeeded - 0 Errors**

## Next Steps (Optional Enhancements)

### 1. Excel Export Implementation
Add NuGet package: `EPPlus` or `ClosedXML`
```csharp
// Example implementation
public async Task<byte[]> ExportToExcel(ReportFilterDto filter)
{
    using var package = new ExcelPackage();
    
    // Sheet 1: Service Performance
    var sheet1 = package.Workbook.Worksheets.Add("Service Performance");
    var serviceData = await GetServicePerformanceReportAsync(filter);
    // Add data to sheet
    
    // Sheet 2: User Productivity
    // Sheet 3: Late Tasks
    // Sheet 4: Project Report
    
    return package.GetAsByteArray();
}
```

### 2. Seed Default Service Rates
Create a data seeding script:
```csharp
var defaultRates = new[]
{
    new ServiceRate { ServiceType = "Translation", Role = "Translator", WordsPerHour = 200 },
    new ServiceRate { ServiceType = "Translation", Role = "Senior Translator", WordsPerHour = 275 },
    new ServiceRate { ServiceType = "Translation", Role = "Reviewer", WordsPerHour = 300 },
    // Add more...
};
```

### 3. Add Charts/Visualizations
- Performance trends over time
- User productivity comparison charts
- Service type distribution pie charts

### 4. Scheduled Reports
- Email reports daily/weekly/monthly
- Automated report generation

### 5. Export Formats
- PDF export
- CSV export
- JSON export

## Usage Instructions

### 1. Access Reports
Navigate to `/reports` in the application

### 2. Configure Service Rates
Use the Service Rates API to set productivity rates:
```bash
POST /api/servicerates
{
  "serviceType": "Translation",
  "role": "Translator",
  "wordsPerHour": 200
}
```

### 3. Generate Reports
1. Select date range
2. Apply filters (optional)
3. Click "Generate Reports"
4. Switch between tabs to view different reports

### 4. Export Data
Click "Export to Excel" to download all reports (when implemented)

## Testing Checklist

- [ ] Create service rates for different service types
- [ ] Create tasks with different service types
- [ ] Assign tasks to translators, reviewers, team leaders
- [ ] Complete tasks (set CompletedDate)
- [ ] Approve tasks (set Status = Approved)
- [ ] Add delay reasons for late tasks
- [ ] Generate reports with different filters
- [ ] Verify calculations are correct
- [ ] Test Excel export (when implemented)
- [ ] Test responsive design on mobile

## Performance Considerations

- Reports use async/await for non-blocking operations
- Filtering happens at the service layer
- Consider adding caching for frequently accessed reports
- Add pagination for large datasets
- Consider adding indexes on frequently queried fields:
  - `TranslationTask.ServiceType`
  - `TranslationTask.ProjectId`
  - `TranslationTask.Status`
  - `TranslationTask.CompletedDate`

## Notes

- All dates are stored in UTC
- Performance percentage > 100% means user completed faster than expected
- Performance percentage < 100% means user took longer than expected
- Tasks must be in "Approved" status to appear in reports
- Delay reasons are tracked in TaskHistory
