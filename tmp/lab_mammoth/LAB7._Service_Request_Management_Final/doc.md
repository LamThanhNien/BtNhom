__LAB 7\. Service Request Management __

1. __Enabling Redis Memory Caching__

__B1\. Cài chocolate__

__Mở cmd với quyền admin__

__>__ @"%SystemRoot%\\System32\\WindowsPowerShell\\v1\.0\\powershell\.exe" \-NoProfile \-InputFormat None \-ExecutionPolicy Bypass \-Command "iex \(\(New\-Object System\.Net\.WebClient\)\.DownloadString\('https://chocolatey\.org/install\.ps1'\)\)" && SET "PATH=%PATH%;%ALLUSERSPROFILE%\\chocolatey\\bin"

>choco \-v

__![](images/img_001.png)__

__B2\. Cài Redis__

__               >__ choco install redis\-64

	      Run Redis 

	     >redis\-server

	     ![](images/img_002.png)

	   Chú ý port kết nối với redis là 6379 qua localhost

__B3\. Cấu hình Cache với Redis__

__Mở nuget cài __Microsoft\.Extensions\.Caching\.StackExchangeRedis trong tầng ASC\.Web

__![](images/img_003.png)__

__Cập nhật cấu hình hình kết nối với server redis__

__ASC\.Web/__ __appsettings\.json__

__![](images/img_004.png)__

__Add DistributedRedisCache trong method AddCongfig thuộc lớp DependencyInjection__

__![](images/img_005.png)__

__Tạo model  __MasterDataCache để lưu trữ MasterData trong bộ nhớ đêm redis

__![](images/img_006.png)__

__Tạo intefarce IMasterDataCacheOperations\.cs__

__![](images/img_007.png)__

__Tạo class __MasterDataCacheOperations\.cs

__![](images/img_008.png)__

__Add service __MasterDataCacheOperations__ trong method __AddMyDependencyGroup__ thuộc lớp DependencyInjection__

__![](images/img_009.png)__

__Cập class Program\.cs create MasterDataCache__

__![](images/img_010.png)__

__	      Kiểm tra kết nối lưu trữ bộ nhớ đệm trên redis__

__		>redis\-cli__

__		Truy cập quyền admin \-> Master Data  \-> Master Key__

__		>__ __keys \*__

__		![](images/img_011.png)__

__		Xem bộ nhớ đệm tương ứng__

__		> hgetall ASCInstanceMasterDataCache__

__		![](images/img_012.png)__

__	Chú ý: đóng console redis có có thể close và app lấy dữ liệu từ 2 bộ nhớ đệm ASCInstanceMasterDataCache, … \. Nên phải start redis trước\.__

1. __Service Request Workflow__

__![](images/img_013.png)__

1. __New Service Request \(Role Customer\)__

__B1\. Tạo mới model __ServiceRequest __trong ASC\.Model\.Models__

__![](images/img_014.png)__

Mở Package Manager Console

PM> Add\-Migration addServiceRequest

PM> Update\-Database –Verbose

__B2\. Tạo interface IserviceRequestOperations và class ServiceRequestOperations trong ASC\.Business__

__![](images/img_015.png)__

![](images/img_016.png)

![](images/img_017.png)

Inject __ServiceRequestOperations vào ASC\.Web => Cập nhật method __AddMyDependencyGroup trong class DependencyInjection\.cs

![](images/img_018.png)

__B3\. Xử lý User Case  New Service Request \(Role Customer\) trong ASC\.Web\.__ __Areas\.__ __ServiceRequests__

__Tạo Model NewServiceRequestViewModel\.cs__

__![](images/img_019.png)__

__Tạo Model __ServiceRequestMappingProfile\.cs

__![](images/img_020.png)__

__Tạo controller ServiceRequestController\.cs__

__![](images/img_021.png)__

__![](images/img_022.png)__

__Tạo view cho action __

__![](images/img_023.png)__

__Tạo layout ASC\.Web\.Areas/ServiceRequests/View/__ __\_ViewImports\.cshtml__

__![](images/img_024.png)__

__Tạo view ServiceRequest\.cshtml__

__![](images/img_025.png)__

__![](images/img_026.png)__

__Thực hiện login với Role Customer \-> Service Requests \-> New Service Request __

__![](images/img_027.png)__

__![](images/img_028.png)__

__Trường hợp dữ liệu không hợp lệ__

__![](images/img_029.png)__

__Khi thêm mới thành công check CSDL__

__![](images/img_030.png)__

1. __Dashboard Service Request__

- __Tạo class PredicateBuilder\.cs để tạo các biểu thức lọc  với AND hoặc OR của LINQ__

__![](images/img_031.png) __

- __Tạo class __Queries\.cs __để tạo biểu thức truy vấn LINQ về các yêu cầu dịch vụ cho các phân quyền khác nhau \( Admin, Service Engerneering, Customer\)__

__![](images/img_032.png)__

__Cập nhật __IRepository\.cs

__![](images/img_033.png)__

__Cập nhật __Repository\.cs

![](images/img_034.png)

__Cập nhật interface __IServiceRequestOperations\.cs__ ![](images/img_035.png)__

__Cập nhật class ServiceRequestOperations\.cs override phương thức__

![](images/img_036.png)

__Tạo model cho view DashboardViewModel\.cs__

__![](images/img_037.png)__

__Cập nhật DashboardController trong Area ServiceRequests__

__![](images/img_038.png)__

__![](images/img_039.png)__

__![](images/img_040.png)__

__Tạo \(Layout\) Partial view  \\Areas\\ServiceRequest\\Views\\Shared\\\_ServiceRequestGrid\.cshtml__

__![](images/img_041.png)__

__![](images/img_042.png)__

__![](images/img_043.png)__

__![](images/img_044.png)__

__Cập nhật view Dashboard cho action Dashboard__

__![](images/img_045.png)__

__![](images/img_046.png)__

__Kết quả chạy role \(Customer\)__

__![](images/img_047.png)__

__Kết quả chạy role \(Admin\)__

__![](images/img_048.png)__

__	Kết quả chạy role \(Engerneering\)__

__![](images/img_049.png)__

1. __Dashboard Service Request \(Role Engineer\)__

