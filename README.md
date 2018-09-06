# JrtFS
A simple but useful file/image server 
Base on .net C# and asp.net WebApi

这是一个简单易用的文件服务器，部署成本基本为零，非常适合访问量不大的系统，或者项目初期使用，
支持Url图片处理服务，如：图片缩放、裁剪、旋转、添加文字、水印等

提供一套完整操作的RESTful API可以跨平台调用
提供.net端 Api调用操作文件

Bucket
存储在JrtFS上的每个文件必须都包含在Bucket中，Bucket名字在整个OSS中具有全局唯一性，且不能修改。一个应用，例如图片分享网站，可以对应一个或多个Bucket，每个Bucket中存放的Object的数量和大小总和没有限制，用户不需要考虑数据的可扩展性。

接口安全
调用接口需要使用授权accessKeyId、accessKeySecret配合调用
