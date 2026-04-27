__LAB 6\. Master Data Management \(Roles Admin\)__

1. __Tầng ASC\.Model__

__![](images/img_001.png)__

1. __Tầng ASC\.DataAccess__

__![](images/img_002.png)__

1. __Tầng ASC\.__ __Business__

__Tạo references projects __

__![](images/img_003.png)__

__/Interfaces/__ __ImasterDataOperations\.cs__

__![](images/img_004.png)__

__//__ MasterDataOperations\.cs

__![](images/img_005.png)__

__![](images/img_006.png)__

__![](images/img_007.png)__

__![](images/img_008.png)__

__![](images/img_009.png)__

1. __Tầng ASC\.Web__
	1. __User Case Master Data Management__

__B1\. Sử dụng AutoMapper để ánh xạ các model từ layer  ASC\.Business với các model\. Cài AutoMapper\.Extensions\.Microsoft\.DependencyInjection bằng nuget cho ASC\.Web__

__![](images/img_010.png)__

__B2\. Tạo Areas Configuration: xử lý thêm dữ liệu cấu hình cho dự án__

__![](images/img_011.png)__

__B3\. Tạo các models__

__	//__ __MasterDataKeyViewModel\.cs__

__	![](images/img_012.png)__

__      // MasterKeysViewModel\.cs__

__	![](images/img_013.png)__

__	//__ __MasterDataValueViewModel\.cs__

__	![](images/img_014.png)__

__//MasterValuesViewModel\.cs__

__![](images/img_015.png)__

__//MappingProfile\.cs__

__![](images/img_016.png)__

__	     B4\. Add  __AddMyDependencyGroup in DependencyInjection

public static IServiceCollection AddMyDependencyGroup\(this IServiceCollection services\)

 \{

    …\.

     //Add MasterDataOperations

     services\.AddScoped<IMasterDataOperations, MasterDataOperations>\(\);

     services\.AddAutoMapper\(typeof\(ApplicationDbContext\)\);

     //

     return services;

 \}

	

__B5\. Controller /Configuration/Controllers/MasterDataController__

__![](images/img_017.png)__

__![](images/img_018.png)__

__//Master Value__

__![](images/img_019.png)__

__![](images/img_020.png)__

__//Upload excel__

__![](images/img_021.png)__

__![](images/img_022.png)__

__B5\. Tạo View cho action MasterKeys\.cshtml__

__![](images/img_023.png)__

__![](images/img_024.png)__

__![](images/img_025.png)__

__B6\. Tạo View cho action MasterValues\.cshtml__

__![](images/img_026.png)__

__![](images/img_027.png)__

__![](images/img_028.png)__

__![](images/img_029.png)__

__![](images/img_030.png)__

__![](images/img_031.png)__

__![](images/img_032.png)__

__![](images/img_033.png)__

__![](images/img_034.png)__

__![](images/img_035.png)__

__B6\. Chạy dự án__

__//Giao diện masterkey__

__![](images/img_036.png)__

__![](images/img_037.png)__

__//Giao diện mastervalue__

__![](images/img_038.png)__

