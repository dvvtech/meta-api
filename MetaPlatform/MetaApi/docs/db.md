Если нужно заново создать базу.

- удаляем текущую базу
- удаляем папку Migration
- создаем новую миграцию
- добавляем старые данные


Package Manager Console
DropDown -> Data Project
Startup project -> API Project

Add-Migration InitialCreate
Update-Database



INSERT [dbo].[Accounts] ([ExternalId], [UserName], [JwtRefreshToken], [AuthType], [Role], [IsBlocked], [CreatedUtcDate], [UpdateUtcDate]) VALUES (N'103864177264690986778', N'Владимир', N'kTXAwJF0pNKwFy9r+1CJqVMJ/YE0q1WxhmJ8NuiuAsIDWjj3nQ8NxIc02O6LTCXo5q9qCnTpyf5BSvBP5dmeyw==', 2, 1, 0, CAST(N'2025-04-07T14:37:19.5009610' AS DateTime2), CAST(N'2025-04-08T20:53:55.3333333' AS DateTime2))


USE [db_1baa3]
GO
SET IDENTITY_INSERT [dbo].[FittingResults] ON 
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/oldv2/woman/3c9d3cfc-cad9-47d8-bf8f-52ee2f6f44f0_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/woman/effba013-376c-420e-bcc9-1a6423eedf4b_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/result/563117e4-9ca2-4460-9e02-c154587ef137_t.jpg', 1, CAST(N'2024-12-17T07:50:37.6363506' AS DateTime2), 1)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/manClothing/74ff9fb1-d895-4a78-81cc-9bf07402ec7f_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/man/075a89eb-1f5b-428b-9304-e16fd32b00a7_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/result/e7cec0e8-eb1b-44c2-aef4-f11e4100d70c_t.jpg', 1, CAST(N'2024-12-17T07:50:37.6363506' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/manClothing/967a25b8-5907-440c-8920-ee4ea473e7bd_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/5e5a421b-bc34-48c3-a90a-d1efa13f4794_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/result/dda6e6a0-86a5-4a93-a3cb-1273775bb9cc_t.jpg', 1, CAST(N'2024-12-17T07:50:37.6363506' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/manClothing/b4e4fc1d-b681-4981-b280-859537eca9ce_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/5e5a421b-bc34-48c3-a90a-d1efa13f4794_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/result/56cdc48b-3107-4fbf-b006-03c6a215e598_t.jpg', 1, CAST(N'2024-12-17T07:50:37.6363506' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/manClothing/e5e275fd-5762-4a63-afa0-01c24b5abdd1_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/5e5a421b-bc34-48c3-a90a-d1efa13f4794_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/result/574d7d66-cb17-43bd-ad69-4e20260cdaf9_t.jpg', 1, CAST(N'2024-12-17T07:50:37.6363506' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/manClothing/ea77d16b-15e2-47eb-afb4-e517ac567351_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/6693fb21-4248-4429-ab3b-a6fd674d29e5_t.jpeg', N'https://a33140-9deb.k.d-f.pw/oldv2/result/55694fea-2249-4a47-88ec-19d5ced97e17_t.jpg', 1, CAST(N'2024-12-18T09:37:52.5086325' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/manClothing/abe50169-aa24-448a-986a-3e4fb347dfdf_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/man/faab15c3-2fab-432b-8e26-05cd5f4d49ee_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/result/47f4c5c1-7941-4501-b63c-4c6ca137aeda_t.jpg', 1, CAST(N'2024-12-18T17:30:59.5141453' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/woman/7be4fab1-8c8d-4f4a-85f1-401850df730a_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/5ce6094c-d1a9-42b0-a13f-79ea7b2b1906_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/result/115cba00-4fca-43da-a13f-69c8756e88a8_t.jpg', 1, CAST(N'2024-12-18T17:41:46.3108751' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/woman/effba013-376c-420e-bcc9-1a6423eedf4b_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/woman/3c9d3cfc-cad9-47d8-bf8f-52ee2f6f44f0_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/result/c4145978-dde8-4266-bf0b-aec6a59627f8_t.jpg', 1, CAST(N'2024-12-18T20:38:57.0810033' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/06c4f54e-be2d-40c6-b62a-56e432aee8b5_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/cbd90c21-1ed1-4659-9a5f-5f1e2d198e23_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/result/c7c8432a-eb2c-4a84-98ac-a9d8534834ab_t.jpg', 1, CAST(N'2024-12-18T21:55:16.9176199' AS DateTime2), 1)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/woman/9c719b52-b8af-4e35-8ab0-f63dd2a4db22_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/woman/23a4596c-24b9-4ead-a764-83c51ceab5b4_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/result/b972f2aa-901b-47da-9e10-6eec2fb2a9c6_t.jpg', 1, CAST(N'2024-12-18T22:13:01.0247374' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/woman/8fdba421-0832-48ea-8054-a82535578a32_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/woman/85983a8d-e655-447d-85d0-5233aec3d332_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/result/8ade4719-9011-4f6d-b9ee-b3c1a2fa68d4_t.jpg', 1, CAST(N'2024-12-19T04:00:09.7187323' AS DateTime2), 1)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/woman/23a4596c-24b9-4ead-a764-83c51ceab5b4_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/woman/9c719b52-b8af-4e35-8ab0-f63dd2a4db22_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/result/b812a4d6-52b5-429d-a9f5-aec685e0437b_t.jpg', 1, CAST(N'2024-12-19T06:46:39.4826941' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/manClothing/74ff9fb1-d895-4a78-81cc-9bf07402ec7f_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/man/7fe33030-6cd7-4824-8f40-5479c80f74f2_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/result/d211d593-59b4-497b-8368-8d13b14f8dc1_t.jpg', 1, CAST(N'2024-12-20T17:54:23.6147778' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/manClothing/967a25b8-5907-440c-8920-ee4ea473e7bd_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/9dba8ea0-47f1-419d-92ec-14628ccb5ae6_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/result/bdc97bff-006e-482b-95d3-a6198d11a411_t.jpg', 1, CAST(N'2024-12-20T17:56:30.1296951' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/man/7fe33030-6cd7-4824-8f40-5479c80f74f2_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/2b0135d9-daba-4fa6-94e8-ca57222b95b9_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/result/90b0752b-087d-43e9-9d52-662fa5786506_t.jpg', 1, CAST(N'2024-12-20T18:03:11.5301095' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/manClothing/7a083247-29d3-47da-9748-b06579017ddc_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/b8ab8772-58aa-47e2-98d0-33d695f7dd37_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/result/05b7924f-488d-40bc-bd96-e14ee84942ba_t.jpg', 1, CAST(N'2024-12-20T18:13:11.1349854' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/manClothing/298796b0-ff00-4375-9341-5149c78b1fd3_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/b8ab8772-58aa-47e2-98d0-33d695f7dd37_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/result/de500800-057e-4682-a69e-207d47e24fa0_t.jpg', 1, CAST(N'2024-12-20T18:14:20.0079618' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/671fbca5-39d1-403b-9aeb-5ce82ec2edd8_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/woman/effba013-376c-420e-bcc9-1a6423eedf4b_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/result/9e063cbc-1a10-48a4-bd0e-8c9820855aa5_t.jpg', 1, CAST(N'2024-12-21T23:24:54.2549523' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/woman/3c9d3cfc-cad9-47d8-bf8f-52ee2f6f44f0_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/6be56231-3d85-4383-82e4-fa33a9ede0e3_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/result/78c15539-4844-4276-bcb7-1b4443e0196c_t.jpg', 1, CAST(N'2024-12-22T00:51:26.1963877' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/woman/3c9d3cfc-cad9-47d8-bf8f-52ee2f6f44f0_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/c5251fb9-9bea-4e42-a2e1-7f04ae7462d6_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/result/a58fa41c-a344-4641-a3a8-e09b1e6fe354_t.jpg', 1, CAST(N'2024-12-22T00:57:48.0125985' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/woman/3c9d3cfc-cad9-47d8-bf8f-52ee2f6f44f0_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/1ce139a4-ce8c-4281-9270-7516555854b4_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/result/0458395d-f7de-4457-83df-b3271e7ca5ec_t.jpg', 1, CAST(N'2024-12-22T01:01:25.1106856' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/manClothing/967a25b8-5907-440c-8920-ee4ea473e7bd_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/58695a99-2076-4acd-9727-4b870a447951_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/result/f35a9cb1-9fe6-489e-8a29-eee08ec8e562_t.jpg', 1, CAST(N'2024-12-22T01:35:42.8090941' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/5f9c5a35-f571-4c72-ba9d-7b7c5030503d_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/58695a99-2076-4acd-9727-4b870a447951_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/result/582eeb31-1f79-4058-bac7-d212c8b34983_t.jpg', 1, CAST(N'2024-12-22T01:54:01.8638490' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/d81b39cc-d16a-41ec-8506-1b2b3f70b757_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/8c4d8179-9b67-418f-bb9a-a5b45ed06352_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/result/ceda0c7e-c242-4fe7-bd0b-0ff051533bbe_t.jpg', 1, CAST(N'2024-12-22T02:07:01.3630819' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/295168a4-c133-428f-acfb-7dc9df4d45c5_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/58695a99-2076-4acd-9727-4b870a447951_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/result/a9d4d438-b37c-41e4-a07e-b3791ed575a4_t.jpg', 1, CAST(N'2024-12-22T02:17:21.9804238' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/295168a4-c133-428f-acfb-7dc9df4d45c5_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/1bf8acd2-9424-4180-b729-70e16a1a53b7_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/result/81522ab7-8de0-4911-8ea8-48b46e256db4_t.jpg', 1, CAST(N'2024-12-22T02:26:01.0084503' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/b0bba630-2a4e-4f41-bf13-73927b5ba4dd_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/1bf8acd2-9424-4180-b729-70e16a1a53b7_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/result/f751f489-872a-4690-9924-9c57e9711ff5_t.jpg', 1, CAST(N'2024-12-22T02:28:30.5173064' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/manClothing/967a25b8-5907-440c-8920-ee4ea473e7bd_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/c3f2baaa-d2ea-442a-ac03-f82dcae0568e_t.jpeg', N'https://a33140-9deb.k.d-f.pw/oldv2/result/24e3681c-cd8a-4644-8c68-ceb04e6949bc_t.jpg', 1, CAST(N'2024-12-22T07:59:35.2424874' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/woman/3c9d3cfc-cad9-47d8-bf8f-52ee2f6f44f0_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/woman/effba013-376c-420e-bcc9-1a6423eedf4b_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/result/a9bc3d5f-dac5-4137-ba07-cc480de23378_t.jpg', 1, CAST(N'2024-12-22T20:29:27.8919921' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/woman/8fdba421-0832-48ea-8054-a82535578a32_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/woman/85983a8d-e655-447d-85d0-5233aec3d332_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/result/ffd7a4b6-faa6-4855-860a-c82d0093cc2f_t.jpg', 1, CAST(N'2024-12-22T20:43:25.5131721' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/womanClothing/98373ac0-5cd9-4e55-9fbf-843cff5125e9_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/woman/effba013-376c-420e-bcc9-1a6423eedf4b_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/result/2b5dfa14-cfc8-496e-9dd0-5cca98795879_t.jpg', 1, CAST(N'2024-12-23T08:10:57.2121213' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/manClothing/967a25b8-5907-440c-8920-ee4ea473e7bd_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/e9bc4787-437a-4ed5-8aea-0218e7187e12_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/result/1874ab80-3095-485f-b5bc-16c55d63ee17_t.jpg', 1, CAST(N'2024-12-25T06:02:55.9324364' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/3ee1a3b3-f045-45ac-b710-a367c8a9fae3_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/man/f5722e69-6ca3-471f-b357-12bf46387d98_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/result/0aac2fba-bf44-4dc4-a240-fa1d630931c3_t.jpg', 1, CAST(N'2024-12-25T09:27:29.4142274' AS DateTime2), 1)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/uploads/3ee1a3b3-f045-45ac-b710-a367c8a9fae3_t.jpg', N'https://a33140-9deb.k.d-f.pw/oldv2/man/f5722e69-6ca3-471f-b357-12bf46387d98_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/result/4d28129d-5b07-4359-b4c3-ea6c540b3e04_t.jpg', 1, CAST(N'2024-12-25T09:35:00.2087718' AS DateTime2), 1)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/woman/22f18c0e-44e8-4bab-aba7-682c849d6076_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/woman/e0e3adc2-3d15-420d-921a-c903e3787657_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/result/5cf8afe2-5a0d-4d25-9697-f047197bb7c4_t.jpg', 1, CAST(N'2024-12-31T05:19:40.6090580' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/woman/22f18c0e-44e8-4bab-aba7-682c849d6076_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/woman/23a4596c-24b9-4ead-a764-83c51ceab5b4_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/result/5f940adf-296c-4249-8cc6-fa1222225002_t.jpg', 1, CAST(N'2024-12-31T05:56:47.8101947' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/oldv2/woman/22f18c0e-44e8-4bab-aba7-682c849d6076_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/woman/effba013-376c-420e-bcc9-1a6423eedf4b_t.png', N'https://a33140-9deb.k.d-f.pw/oldv2/result/e2d54d11-cf86-4e16-bae7-d7c32124d7ef_t.jpg', 1, CAST(N'2024-12-31T06:39:12.2073745' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/woman/e17ed6b4-04f5-4ff4-869d-f06d45a164b7_1.484_t.png', N'https://a33140-9deb.k.d-f.pw/woman/8c4d8641-2373-4e0b-a6a9-64f76a584e47_t.png', N'https://localhost:5023/result/851c96d3-9ded-4abe-9f82-0c305ed8b67f_t.jpg', 1, CAST(N'2025-01-14T07:17:22.2243964' AS DateTime2), 1)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/woman/e17ed6b4-04f5-4ff4-869d-f06d45a164b7_1.484_t.png', N'https://a33140-9deb.k.d-f.pw/woman/84853baa-3bc2-4976-8454-56deded8e5b1_1.553_t.png', N'https://a33140-9deb.k.d-f.pw/result/15780284-7d43-4112-a644-cdb83162d9e9_t.jpg', 1, CAST(N'2025-01-14T07:27:35.5162175' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/woman/e17ed6b4-04f5-4ff4-869d-f06d45a164b7_1.484_t.png', N'https://a33140-9deb.k.d-f.pw/woman/78cbd61d-eb75-47f5-a75d-249728319d62_1.466_t.png', N'https://a33140-9deb.k.d-f.pw/result/126cc193-59e9-4a44-8729-6d5955b5af98_t.jpg', 1, CAST(N'2025-01-14T07:29:40.3096026' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/woman/e17ed6b4-04f5-4ff4-869d-f06d45a164b7_1.484_t.png', N'https://a33140-9deb.k.d-f.pw/woman/cdb8be6a-abf3-4699-a91c-7c825aac8301_1.595_t.png', N'https://a33140-9deb.k.d-f.pw/result/ded4761a-32dd-4d4f-92e0-880ce8f39591_t.jpg', 1, CAST(N'2025-01-14T07:30:53.4211025' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/woman/78cbd61d-eb75-47f5-a75d-249728319d62_1.466_t.png', N'https://a33140-9deb.k.d-f.pw/woman/63b48ece-e420-49f3-a6b2-5489ecbcc8ca_1.492_t.png', N'https://a33140-9deb.k.d-f.pw/result/d7aeb60e-8963-4483-b157-3f6cc02bcea3_t.jpg', 1, CAST(N'2025-01-14T07:35:04.7212764' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/womanClothing/e17e7d57-73ca-4336-9874-27d02faaef6c_1.095_t.png', N'https://a33140-9deb.k.d-f.pw/uploads/d40ec80e-efd8-4ba9-af7f-5b529b09d5ad_t.jpg', N'https://a33140-9deb.k.d-f.pw/result/c8cca101-8cd2-411e-9404-83ace54b18f4_t.jpg', 1, CAST(N'2025-01-14T16:24:31.1242752' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/womanClothing/82e7c6d6-8b41-4e00-ae56-5f38b95327a5_t.png', N'https://a33140-9deb.k.d-f.pw/uploads/d40ec80e-efd8-4ba9-af7f-5b529b09d5ad_t.jpg', N'https://a33140-9deb.k.d-f.pw/result/f6f66e2c-167a-499b-b64c-f43f7fc69166_t.jpg', 1, CAST(N'2025-01-14T16:25:55.7161492' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/womanClothing/067e84b8-5a6c-40e4-9bf3-c08f1ec213b1_t.png', N'https://a33140-9deb.k.d-f.pw/uploads/d40ec80e-efd8-4ba9-af7f-5b529b09d5ad_t.jpg', N'https://a33140-9deb.k.d-f.pw/result/bb58869c-1ae9-44e3-8f56-3259635fb7b9_t.jpg', 1, CAST(N'2025-01-14T16:27:19.3220730' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/womanClothing/067e84b8-5a6c-40e4-9bf3-c08f1ec213b1_t.png', N'https://a33140-9deb.k.d-f.pw/uploads/d40ec80e-efd8-4ba9-af7f-5b529b09d5ad_t.jpg', N'https://a33140-9deb.k.d-f.pw/result/56147f2f-33a1-4061-b07f-d266db147e4d_t.jpg', 1, CAST(N'2025-01-14T16:28:02.0127674' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/woman/e17ed6b4-04f5-4ff4-869d-f06d45a164b7_1.484_t.png', N'https://a33140-9deb.k.d-f.pw/uploads/d40ec80e-efd8-4ba9-af7f-5b529b09d5ad_t.jpg', N'https://a33140-9deb.k.d-f.pw/result/b85d4a87-ccf0-4658-8f90-827c5e6e51e2_t.jpg', 1, CAST(N'2025-01-14T16:30:00.3156358' AS DateTime2), 1)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/woman/63b48ece-e420-49f3-a6b2-5489ecbcc8ca_1.492_t.png', N'https://a33140-9deb.k.d-f.pw/uploads/d40ec80e-efd8-4ba9-af7f-5b529b09d5ad_t.jpg', N'https://a33140-9deb.k.d-f.pw/result/b0cbc25f-d568-4992-841d-cdb7bf58b98d_t.jpg', 1, CAST(N'2025-01-14T16:35:57.6112730' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/manClothing/7b00a03b-fbb9-4296-a624-13fbcee2febc_1.057_t.png', N'https://a33140-9deb.k.d-f.pw/uploads/3ec94f9b-02c2-4c3f-aba0-ba7c258e37bd_1.5_t.jpg', N'https://a33140-9deb.k.d-f.pw/result/f00875b0-0c32-41e7-8bcb-e9a34a791f29_t.jpg', 1, CAST(N'2025-01-14T16:38:18.1121926' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/womanClothing/067e84b8-5a6c-40e4-9bf3-c08f1ec213b1_t.png', N'https://a33140-9deb.k.d-f.pw/uploads/b7135a51-4412-43cf-9cb5-9af7e06b2c28_t.jpg', N'https://a33140-9deb.k.d-f.pw/result/a54032b4-24c6-4f0a-b625-adde81236cb1_t.jpg', 1, CAST(N'2025-01-14T20:43:44.9105791' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/womanClothing/067e84b8-5a6c-40e4-9bf3-c08f1ec213b1_t.png', N'https://a33140-9deb.k.d-f.pw/uploads/6dbafbf9-94b1-4b5d-a97f-676438e82199_0.562_t.jpg', N'https://a33140-9deb.k.d-f.pw/result/6f5af0b2-f88c-45c8-94d6-b41347e4adf4_t.jpg', 1, CAST(N'2025-01-14T20:50:04.0076922' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/uploads/da53b570-c13e-4d61-a143-b10a3a933063_0.544_t.jpg', N'https://a33140-9deb.k.d-f.pw/uploads/6dbafbf9-94b1-4b5d-a97f-676438e82199_0.562_t.jpg', N'https://a33140-9deb.k.d-f.pw/result/ecf0b3d7-dd73-48c4-b1bf-33106245f511_t.jpg', 1, CAST(N'2025-01-14T21:10:43.5139140' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/uploads/69fb6a9d-ed9c-40f6-9409-f5895622f289_1_t.webp', N'https://a33140-9deb.k.d-f.pw/uploads/551a0f0a-fa95-40eb-baf9-708a35e0c8e3_1.501_t.webp', N'https://a33140-9deb.k.d-f.pw/result/b6a83cf4-6080-4ac5-a111-63bbb5ed1382_t.jpg', 1, CAST(N'2025-01-16T16:35:05.7132425' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/uploads/5bf6b397-1a57-4834-bdd0-5d956ea11c41_1.503_t.jpg', N'https://a33140-9deb.k.d-f.pw/uploads/551a0f0a-fa95-40eb-baf9-708a35e0c8e3_1.501_t.webp', N'https://a33140-9deb.k.d-f.pw/result/25db3486-dba2-4d10-ab44-5be7eab66bb3_t.jpg', 1, CAST(N'2025-01-16T16:38:59.2107130' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/uploads/5bf6b397-1a57-4834-bdd0-5d956ea11c41_1.503_t.jpg', N'https://a33140-9deb.k.d-f.pw/uploads/551a0f0a-fa95-40eb-baf9-708a35e0c8e3_1.501_t.webp', N'https://a33140-9deb.k.d-f.pw/result/893185c3-41ee-462d-9d87-773b9547bcff_t.jpg', 1, CAST(N'2025-01-16T16:41:42.0110524' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/uploads/04c39f9c-5974-4786-b9d8-1f6e76d0cada_0.961_t.jpg', N'https://a33140-9deb.k.d-f.pw/uploads/f13e1e3b-61ef-4a99-abfe-e50961ae461d_t.webp', N'https://a33140-9deb.k.d-f.pw/result/abfb17d7-c461-432e-aedb-4da5b3415776_t.jpg', 1, CAST(N'2025-01-16T17:02:46.0181392' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/uploads/5bf6b397-1a57-4834-bdd0-5d956ea11c41_1.503_t.jpg', N'https://a33140-9deb.k.d-f.pw/uploads/f13e1e3b-61ef-4a99-abfe-e50961ae461d_t.webp', N'https://a33140-9deb.k.d-f.pw/result/a46a5d83-ffeb-404e-a005-4093529619c5_t.jpg', 1, CAST(N'2025-01-16T17:03:51.9186068' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/woman/78cbd61d-eb75-47f5-a75d-249728319d62_1.466_t.png', N'https://a33140-9deb.k.d-f.pw/woman/3ac9d1c3-4fae-455b-9bef-30e9a2395a7b_2.433_t.png', N'https://a33140-9deb.k.d-f.pw/result/136adbf3-64d7-46d4-aa7f-894203078159_t.jpg', 1, CAST(N'2025-01-28T22:01:23.0215248' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/womanClothing/e17e7d57-73ca-4336-9874-27d02faaef6c_1.095_t.png', N'https://a33140-9deb.k.d-f.pw/woman/547babf5-f046-4b73-aa2c-a7d4494298d3_1.481_t.png', N'https://a33140-9deb.k.d-f.pw/result/666a21a8-d7a3-4631-af48-4c3a96604ef4_t.jpg', 1, CAST(N'2025-02-13T05:57:46.4099429' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/woman/cdb8be6a-abf3-4699-a91c-7c825aac8301_1.595_t.png', N'https://a33140-9deb.k.d-f.pw/woman/78cbd61d-eb75-47f5-a75d-249728319d62_1.466_t.png', N'https://a33140-9deb.k.d-f.pw/result/f103700b-16eb-42e2-9885-f6724a6c070a_t.jpg', 1, CAST(N'2025-02-13T06:01:34.8087513' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/womanClothing/de419a4d-d232-40a6-b8ca-d065788d1c4c_1.593_t.png', N'https://a33140-9deb.k.d-f.pw/woman/d71dea15-0bed-4f27-82da-0e518918f3c2_1.474_t.png', N'https://a33140-9deb.k.d-f.pw/result/96db34ac-18d3-4886-b833-741957bfc822_t.jpg', 1, CAST(N'2025-02-13T07:27:42.2175142' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/womanClothing/de419a4d-d232-40a6-b8ca-d065788d1c4c_1.593_t.png', N'https://a33140-9deb.k.d-f.pw/woman/63b48ece-e420-49f3-a6b2-5489ecbcc8ca_1.492_t.png', N'https://a33140-9deb.k.d-f.pw/result/00461a45-3690-48ae-b21d-2f7d96157075_t.jpg', 1, CAST(N'2025-02-13T07:28:32.7277218' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/womanClothing/de419a4d-d232-40a6-b8ca-d065788d1c4c_1.593_t.png', N'https://a33140-9deb.k.d-f.pw/woman/e17ed6b4-04f5-4ff4-869d-f06d45a164b7_1.484_t.png', N'https://a33140-9deb.k.d-f.pw/result/35562090-f9fb-4c77-b1d2-f4ccaa645a3d_t.jpg', 1, CAST(N'2025-02-13T08:54:44.2213021' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/womanClothing/de419a4d-d232-40a6-b8ca-d065788d1c4c_1.593_t.png', N'https://a33140-9deb.k.d-f.pw/woman/cdb8be6a-abf3-4699-a91c-7c825aac8301_1.595_t.png', N'https://a33140-9deb.k.d-f.pw/result/a1a84376-464f-4d34-93c6-30bcbcc36b02_t.jpg', 1, CAST(N'2025-02-13T08:55:47.2179311' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/womanClothing/de419a4d-d232-40a6-b8ca-d065788d1c4c_1.593_t.png', N'https://a33140-9deb.k.d-f.pw/woman/84853baa-3bc2-4976-8454-56deded8e5b1_1.553_t.png', N'https://a33140-9deb.k.d-f.pw/result/7fc35b5c-d0f9-49ec-80b3-46c44932721e_t.jpg', 1, CAST(N'2025-02-13T08:56:50.1084417' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/womanClothing/de419a4d-d232-40a6-b8ca-d065788d1c4c_1.593_t.png', N'https://a33140-9deb.k.d-f.pw/woman/84853baa-3bc2-4976-8454-56deded8e5b1_1.553_t.png', N'https://a33140-9deb.k.d-f.pw/result/8b744a0a-6a4e-4361-bc68-e63a13a219eb_t.jpg', 1, CAST(N'2025-02-13T08:59:16.9254762' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/womanClothing/de419a4d-d232-40a6-b8ca-d065788d1c4c_1.593_t.png', N'https://a33140-9deb.k.d-f.pw/woman/0da11463-d388-47fd-8baa-397e4769d8ef_1.601_t.png', N'https://a33140-9deb.k.d-f.pw/result/e80a6f55-a60d-4371-9b13-6f323f1c3c76_t.jpg', 1, CAST(N'2025-02-13T09:01:23.4158493' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/womanClothing/de419a4d-d232-40a6-b8ca-d065788d1c4c_1.593_t.png', N'https://a33140-9deb.k.d-f.pw/woman/9cf14ec4-067f-4108-be9a-45e3bb791c54_1.481_t.png', N'https://a33140-9deb.k.d-f.pw/result/ad136272-4de6-40dc-80bd-35c1216de4d4_t.jpg', 1, CAST(N'2025-02-13T09:04:56.4093672' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/womanClothing/de419a4d-d232-40a6-b8ca-d065788d1c4c_1.593_t.png', N'https://a33140-9deb.k.d-f.pw/woman/3ac9d1c3-4fae-455b-9bef-30e9a2395a7b_2.433_t.png', N'https://a33140-9deb.k.d-f.pw/result/9b9ad00a-47b9-48e8-b960-b73fffc6eabe_t.jpg', 1, CAST(N'2025-02-13T09:07:31.7171781' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/womanClothing/de419a4d-d232-40a6-b8ca-d065788d1c4c_1.593_t.png', N'https://a33140-9deb.k.d-f.pw/woman/8c4d8641-2373-4e0b-a6a9-64f76a584e47_t.png', N'https://a33140-9deb.k.d-f.pw/result/57f955ac-ce16-4ebd-9ccb-d74faf33b479_t.jpg', 1, CAST(N'2025-02-13T09:09:36.9074951' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/womanClothing/9ac6253f-90b6-4789-a880-f88a5b17d9b5_1.125_t.png', N'https://a33140-9deb.k.d-f.pw/woman/8c4d8641-2373-4e0b-a6a9-64f76a584e47_t.png', N'https://a33140-9deb.k.d-f.pw/result/fcc3610d-c950-4e23-aeba-10e835730d79_t.jpg', 1, CAST(N'2025-02-13T09:11:28.3199604' AS DateTime2), 0)
GO
INSERT [dbo].[FittingResults] ([GarmentImgUrl], [HumanImgUrl], [ResultImgUrl], [AccountId], [CreatedUtcDate], [IsDeleted]) VALUES (N'https://a33140-9deb.k.d-f.pw/woman/63b48ece-e420-49f3-a6b2-5489ecbcc8ca_1.492_t.png', N'https://a33140-9deb.k.d-f.pw/woman/8c4d8641-2373-4e0b-a6a9-64f76a584e47_t.png', N'https://a33140-9deb.k.d-f.pw/result/ca3e7bed-4d09-4964-a3cd-99e5e84106f1_t.jpg', 1, CAST(N'2025-02-13T09:12:36.9112816' AS DateTime2), 0)
GO
SET IDENTITY_INSERT [dbo].[FittingResults] OFF
GO
