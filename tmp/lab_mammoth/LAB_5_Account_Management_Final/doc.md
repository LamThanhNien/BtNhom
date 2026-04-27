__LAB 5\. Account Management \( Roles Admin\)__

1. __User Case Dang Nhap – role customer  : __

__cho phép người dùng tạo tài khoản và đăng nhập qua Gmail\. Sử dụng oauth 2\.0 aps\.net core__

__	![](images/img_001.png)__

- [__Link tham khảo__](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/?view=aspnetcore-9.0&tabs=visual-studio)
- __Truy cập link và đăng nhập với gmail__

[__https://console\.cloud\.google\.com/projectselector2/apis/library?invt=AbtwFA__](https://console.cloud.google.com/projectselector2/apis/library?invt=AbtwFA)

__Chọn create new project__

__![](images/img_002.png)__

- __Truy cập dự án vào tạo 1 Credentials  \-> Create credentials \-> Oauth Client ID__

__![](images/img_003.png)__

__![](images/img_004.png)__

__![](images/img_005.png)__

__![](images/img_006.png)__

- __Cập nhập appsettings\.json__

__![](images/img_007.png)__

- __Sử dụng nuget cài :__ __Microsoft\.AspNetCore\.Authentication\.Google__

__ __![](images/img_008.png)

- __Cấu hình dịch vụ:__

__![](images/img_009.png)__

- __Cập nhật view login  cho phép đăng nhập và tạo tài khoản khách hàng__

__\./Areas/Identity/Pages/Account/Login\.cshtml__

__![](images/img_010.png)__

- __Cập nhật view /Areas/Identity/Pages/Account/ExternalLogin\.cshtml__

__![](images/img_011.png)__

- __Cập nhật code xử lý /Areas/Identity/Pages/Account/ExternalLogin\.cs__

__![](images/img_012.png)__

__//Model__

__![](images/img_013.png)__

__//Actions__

__![](images/img_014.png)__

__![](images/img_015.png)__

__![](images/img_016.png)__

- __Chạy ứng dụng => Click google __

__![](images/img_017.png)__

__![](images/img_018.png)__

__Trường hợp chưa đăng ký tài khoản qua gmail, nếu email đăng ký tài khoản Admin hoặc Engeering thông báo lỗi__

__![](images/img_019.png)__

__Trường hợp chưa đăng ký tài khoản qua gmail và email chưa dùng đăng ký tài khoản Admin hoặc Engeering\. Sẽ tạo tài khoản cho User và chuyển sang dashboard của người dùng__

__![](images/img_020.png)__

1. __Use Case Service Engineer Dashboard  \- Role Admin: quản lý tài khoản nhân viên sửa chữa\. Cho phép thêm mới tài khoản nhân viên sửa chữa\. Cập nhật thông tin nhân viên sửa chữa\. Trường hợp nhân viên không làm thì sẽ deactive tài khoản \( không xóa tài khoản\)\. Khi cập nhật thông tin tài khoản phải gửi thống báo đến nhân viên qua email__

__B1\. Tạo Area Accounts__

__![](images/img_021.png)__

__Tạo layout /Areas/Accounts/Views/\_ViewImports\.cshtml cho phép import @addTagHelper \*, Microsoft\.AspNetCore\.Mvc\.TagHelpers__

__![](images/img_022.png)__

__B2\. Tạo các class Model__

- class ServiceEngineerRegistrationViewModel : lưu trữ thông tin ServiceEngineer

__![](images/img_023.png)__

- class ServiceEngineerViewModel

__![](images/img_024.png)__

__B3\. Tạo controller  AccountController\.cs__

__	![](images/img_025.png)__

__//Action __ServiceEngineers \(httpGet\) hiện thị danh sách nhân viên

__![](images/img_026.png)__

__//Action __ServiceEngineers \(httpPost\) xử lý cập nhật và thêm mới nhân viên

__![](images/img_027.png)__

__![](images/img_028.png)__

__B4\. Tạo view cho action __ServiceEngineers\.cshtml

__![](images/img_029.png)__

- __Cập nhật \_MasterLayout sử dụng datatables__

<link rel="stylesheet" type="text/css" href="https://cdn\.datatables\.net/1\.11\.3/css/jquery\.dataTables\.css">

<script type="text/javascript" charset="utf8" src="https://cdn\.datatables\.net/1\.11\.3/js/jquery\.dataTables\.js"></script>

- __Cập nhật wwwroot/css/__ __style\.css__

__![](images/img_030.png)__

- __Cập nhật view ServiceEngineers\.cshtml__

__//Giao diện hiển thị danh sách nhân viên__

__![](images/img_031.png)__

__//Giao diện cập nhật và thêm mới nhân viên__

__![](images/img_032.png)__

__//Script xử lý: datatable, cập nhật , thêm mới nhân viên và reset form__

__![](images/img_033.png) __

- __Chạy ứng dụng với quyền admin \-> User Administrator \-> Service Engineers__

__![](images/img_034.png)__

1. __Use Case Customer Dashboard – Role Admin : quản lý tài khoản người dùng\. Hiện thị danh sách người dùng\. Và cho phép cập nhật trạng thái tài khoản người dùng active hoặc deactive\. Khi cập nhật thông tin tài khoản phải gửi thống báo đến người dùng qua email\.__

__B1\. Tạo model__

- class CustomerRegistrationViewModel : lưu trữ thông tin khách hàng đăng ký

__![](images/img_035.png)__

- class CustomerViewModel : lưu trữ danh sách khách hàng và khách hàng đăng ký

__![](images/img_036.png)__

__B2\. Cập nhật Account  controller __

- __Thêm mới action __Customers \(httpGet\) để lấy danh sách khách hàng

__![](images/img_037.png)__

- __Thêm mới action __Customers \(httpPost\) để cập nhật trạng thái khách hàng sang active và deactive 

__Chú ý: không có action xử lý thêm mới khách hàng\. Mà khách hàng phải đăng ký qua giao diện người dùng__

__![](images/img_038.png)__

__B3\. Tạo view cho action Customers\.cshtml__

__![](images/img_039.png)__

__//Hiển thị danh sách khách hàng__

__![](images/img_040.png)__

__//View cập nhật khách hàng__

__![](images/img_041.png)__

__//Script xử lý các sự kiện__

__![](images/img_042.png)__

__B4\. Chạy ứng dụng role admin \-> User Aministration \-> Customers__

__Thực hiện cập nhật trạng thái của user__

__![](images/img_043.png)__

1. __User Case Update Profile: thực hiện cập nhật username cho người dùng__

__B1\. Tạo các model__

__/Areas/Accounts/Models/ProfileModel\.cs__

__![](images/img_044.png)__

__B2\.  Thêm mới Action Get  Profile  trong /Areas/Accounts/Controllers/__ __AccountController\.cs__

__![](images/img_045.png)__

__B3\. Tạo view cho action Profile__

__/Areas/Accounts/Views/__ __Account/Profile\.cshtml__

__![](images/img_046.png)__

__B4\.  Thêm mới Action Post  Profile  trong /Areas/Accounts/Controllers/__ __AccountController\.cs__

__![](images/img_047.png)__

__B5\. Thực hiện chạy và cập nhật Profile__

__![](images/img_048.png)__

