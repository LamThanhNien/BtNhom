__Lab 2\. Domain\-Driven Architecture__

__Repository Partern \- UnitOfWork Partern__

1. __Thiết kế dự án theo kiến trúc sau __
	- __Domain\-Driven Architecture \-  Layered Architecture\)__

__![](images/img_001.png)__

- 
	- __Tầng ASC\.business – Class Library \.NET 8 truy cập DataAccess  và Model: chứa các class xử lý nghiệm vụ của hệ thống  __

__![](images/img_002.png)__

- 
	- __Tầng ASC\.DataAccess – Class Library \.NET 8 truy cập Model và Utilities: chứa các class thao tác và truy vấn cơ sở dữ liệu__

__![](images/img_003.png)__

- 
	- __Tầng ASC\.Model – Class Library \.NET 8 : chứa các class model tương ứng table trong CSDL\.__

![](images/img_004.png)

__	__Sử dụng Nuget để cài pakage EntiFrameworkCore

	![](images/img_005.png)

- 
	- __Tầng ASC\.Utilities – Class Library \.NET 8 : chứa các class xử lý tiện ích__

__![](images/img_006.png)__

Sử dụng Nuget để cài pakage Newtonsoft

- 
	- __ASC\.Web \- Asp\.Net MVC \.NET 8 : tầng web server truy cập Business, DataAccess, Model, Utilities__

__![](images/img_007.png)__

- 
	- __Right click project \-> add \-> Project Reference__

__![](images/img_008.png)__

1. __Code tầng ASC\.Model__
	- class BaseEntity

__![](images/img_009.png)__

- 
	- class Constants

__![](images/img_010.png)__

- 
	- interface IauditTracker

__![](images/img_011.png)__

- 
	- __Tạo các class Model__

__![](images/img_012.png)__

- 
	- class ServiceRequest

__![](images/img_013.png)__

- 
	- Class MasterDataKey

__![](images/img_014.png)__

- 
	- class MasterDataValue

__![](images/img_015.png)__

1. __Code tấng ASC\.DataAccess \- Repository Partern \- UnitOfWork Partern__

__![](images/img_016.png)__

- 
	- interface Irepository

__![](images/img_017.png)__

- 
	- class Repository

__![](images/img_018.png)__

- 
	- interface IunitOfWork

__![](images/img_019.png)__

- 
	- class UnitOfWork

__![](images/img_020.png)__

1. __Code tầng ASC\.Web__
	- __Cấu hình appsetting\.json__

__![](images/img_021.png)__

- 
	- class ApplicationSettings

__![](images/img_022.png)__

- 
	- class ApplicationDbContext

__![](images/img_023.png)__

- 
	- interface IIdentitySeed

__![](images/img_024.png)__

- 
	- class IdentitySeed

__![](images/img_025.png)__

__![](images/img_026.png)__

1. __Thực hiện migration__

__Vào search \-> Feature Search \-> Package Manager Console__

__Thực hiện add migration đầu tiên cho cơ sở dữ liệu__

__PM> Add\-Migration InitialCreate__

__Lệnh trên sẽ tạo ra history migrations cho CSDL__

__![](images/img_027.png)__

__Và mỗi lần cập nhật model thực hiện lệnh này sẽ tạo ra class gồm 2 methob Up va Down__

__![](images/img_028.png)__

__Để cập nhật CSDL sử dụng lệnh sau:__

__PM> __ Update\-Database  \-Verbose

__Hoặc chạy ứng dụng web, CSDL sẽ được cập nhật qua lệnh__

__	![](images/img_029.png)__

1. __Cập nhật class Program\.cs__

__![](images/img_030.png)__

__//Add IdentitySeed và UnitOfWork__

__![](images/img_031.png)__

__	//Config đưa dữ liệu mẫu từ appsetting\.jon lên CSDL__

__	![](images/img_032.png)__

__Yêu cầu: sau khi chạy ứng dụng sẽ khởi tạo Database tương ứng__

![](images/img_033.png)

- Và đọc dữ liệu cấu hình từ file appsetting\.json đưa vào các bảng

![](images/img_034.png)

![](images/img_035.png)

![](images/img_036.png)

![](images/img_037.png)

- __Thực hiện thêm model Product và cập nhật lại CSDL__

__![](images/img_038.png)__

