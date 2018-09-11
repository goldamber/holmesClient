﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppEnglish.EngServRef {
    using System.Runtime.Serialization;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ServerData", Namespace="http://schemas.datacontract.org/2004/07/Server.Service")]
    public enum ServerData : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Video = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Book = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        User = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Role = 3,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        VideoCategory = 4,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        BookCategory = 5,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Word = 6,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        WordForm = 7,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        WordCategory = 8,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Translation = 9,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Definition = 10,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Author = 11,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Game = 12,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Example = 13,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Bookmark = 14,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Group = 15,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PropertyData", Namespace="http://schemas.datacontract.org/2004/07/Server.Service")]
    public enum PropertyData : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Name = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Surname = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Login = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Role = 3,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        RolesName = 4,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Description = 5,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Path = 6,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        IsAbsolute = 7,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        SubPath = 8,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Imgpath = 9,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Mark = 10,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Created = 11,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Date = 12,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Position = 13,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ScoreCount = 14,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Password = 15,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Level = 16,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Year = 17,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        PastForm = 18,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        PastThForm = 19,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        PluralForm = 20,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Category = 21,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Categories = 22,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Author = 23,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Authors = 24,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Synonyms = 25,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Translation = 26,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Translations = 27,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Definition = 28,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Definitions = 29,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Group = 30,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Groups = 31,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="FilesType", Namespace="http://schemas.datacontract.org/2004/07/Server.Service")]
    public enum FilesType : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Video = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Avatar = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        BookImage = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        WordImage = 3,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        VideoImage = 4,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Book = 5,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Subtitles = 6,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="EngServRef.IEngService", SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface IEngService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/AddUser", ReplyAction="http://tempuri.org/IEngService/AddUserResponse")]
        System.Nullable<int> AddUser(string login, string pswd, string avatar, string role, int level);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/AddUser", ReplyAction="http://tempuri.org/IEngService/AddUserResponse")]
        System.Threading.Tasks.Task<System.Nullable<int>> AddUserAsync(string login, string pswd, string avatar, string role, int level);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/AddVideo", ReplyAction="http://tempuri.org/IEngService/AddVideoResponse")]
        System.Nullable<int> AddVideo(string name, string desc, string path, string sub, string img, bool absolute, System.Nullable<int> mark, System.Nullable<int> user, System.Nullable<int> year, System.DateTime created);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/AddVideo", ReplyAction="http://tempuri.org/IEngService/AddVideoResponse")]
        System.Threading.Tasks.Task<System.Nullable<int>> AddVideoAsync(string name, string desc, string path, string sub, string img, bool absolute, System.Nullable<int> mark, System.Nullable<int> user, System.Nullable<int> year, System.DateTime created);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/AddBook", ReplyAction="http://tempuri.org/IEngService/AddBookResponse")]
        System.Nullable<int> AddBook(string name, string desc, string path, string img, bool absolute, System.Nullable<int> mark, System.Nullable<int> user, System.Nullable<int> year, System.DateTime created);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/AddBook", ReplyAction="http://tempuri.org/IEngService/AddBookResponse")]
        System.Threading.Tasks.Task<System.Nullable<int>> AddBookAsync(string name, string desc, string path, string img, bool absolute, System.Nullable<int> mark, System.Nullable<int> user, System.Nullable<int> year, System.DateTime created);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/AddAuthor", ReplyAction="http://tempuri.org/IEngService/AddAuthorResponse")]
        System.Nullable<int> AddAuthor(string name, string surname);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/AddAuthor", ReplyAction="http://tempuri.org/IEngService/AddAuthorResponse")]
        System.Threading.Tasks.Task<System.Nullable<int>> AddAuthorAsync(string name, string surname);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/AddCategory", ReplyAction="http://tempuri.org/IEngService/AddCategoryResponse")]
        System.Nullable<int> AddCategory(string name, AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/AddCategory", ReplyAction="http://tempuri.org/IEngService/AddCategoryResponse")]
        System.Threading.Tasks.Task<System.Nullable<int>> AddCategoryAsync(string name, AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/AddItemCategory", ReplyAction="http://tempuri.org/IEngService/AddItemCategoryResponse")]
        void AddItemCategory(int item, int cat, AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/AddItemCategory", ReplyAction="http://tempuri.org/IEngService/AddItemCategoryResponse")]
        System.Threading.Tasks.Task AddItemCategoryAsync(int item, int cat, AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/AddBookAuthor", ReplyAction="http://tempuri.org/IEngService/AddBookAuthorResponse")]
        void AddBookAuthor(int bookId, int author);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/AddBookAuthor", ReplyAction="http://tempuri.org/IEngService/AddBookAuthorResponse")]
        System.Threading.Tasks.Task AddBookAuthorAsync(int bookId, int author);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/EditData", ReplyAction="http://tempuri.org/IEngService/EditDataResponse")]
        void EditData(int id, string changes, AppEnglish.EngServRef.ServerData data, AppEnglish.EngServRef.PropertyData property);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/EditData", ReplyAction="http://tempuri.org/IEngService/EditDataResponse")]
        System.Threading.Tasks.Task EditDataAsync(int id, string changes, AppEnglish.EngServRef.ServerData data, AppEnglish.EngServRef.PropertyData property);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/EditMark", ReplyAction="http://tempuri.org/IEngService/EditMarkResponse")]
        void EditMark(int id, int usersId, int changes, AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/EditMark", ReplyAction="http://tempuri.org/IEngService/EditMarkResponse")]
        System.Threading.Tasks.Task EditMarkAsync(int id, int usersId, int changes, AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/Upload", ReplyAction="http://tempuri.org/IEngService/UploadResponse")]
        bool Upload(byte[] file, string name, AppEnglish.EngServRef.FilesType type);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/Upload", ReplyAction="http://tempuri.org/IEngService/UploadResponse")]
        System.Threading.Tasks.Task<bool> UploadAsync(byte[] file, string name, AppEnglish.EngServRef.FilesType type);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/Download", ReplyAction="http://tempuri.org/IEngService/DownloadResponse")]
        byte[] Download(string name, AppEnglish.EngServRef.FilesType type);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/Download", ReplyAction="http://tempuri.org/IEngService/DownloadResponse")]
        System.Threading.Tasks.Task<byte[]> DownloadAsync(string name, AppEnglish.EngServRef.FilesType type);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/Delete", ReplyAction="http://tempuri.org/IEngService/DeleteResponse")]
        void Delete(string name, AppEnglish.EngServRef.FilesType type);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/Delete", ReplyAction="http://tempuri.org/IEngService/DeleteResponse")]
        System.Threading.Tasks.Task DeleteAsync(string name, AppEnglish.EngServRef.FilesType type);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/GetItemProperty", ReplyAction="http://tempuri.org/IEngService/GetItemPropertyResponse")]
        string GetItemProperty(int id, AppEnglish.EngServRef.ServerData data, AppEnglish.EngServRef.PropertyData property);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/GetItemProperty", ReplyAction="http://tempuri.org/IEngService/GetItemPropertyResponse")]
        System.Threading.Tasks.Task<string> GetItemPropertyAsync(int id, AppEnglish.EngServRef.ServerData data, AppEnglish.EngServRef.PropertyData property);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/GetMark", ReplyAction="http://tempuri.org/IEngService/GetMarkResponse")]
        System.Nullable<int> GetMark(int itemId, int userId, AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/GetMark", ReplyAction="http://tempuri.org/IEngService/GetMarkResponse")]
        System.Threading.Tasks.Task<System.Nullable<int>> GetMarkAsync(int itemId, int userId, AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/GetItems", ReplyAction="http://tempuri.org/IEngService/GetItemsResponse")]
        int[] GetItems(AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/GetItems", ReplyAction="http://tempuri.org/IEngService/GetItemsResponse")]
        System.Threading.Tasks.Task<int[]> GetItemsAsync(AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/GetFItems", ReplyAction="http://tempuri.org/IEngService/GetFItemsResponse")]
        int[] GetFItems(string filter, AppEnglish.EngServRef.ServerData data, AppEnglish.EngServRef.PropertyData fil);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/GetFItems", ReplyAction="http://tempuri.org/IEngService/GetFItemsResponse")]
        System.Threading.Tasks.Task<int[]> GetFItemsAsync(string filter, AppEnglish.EngServRef.ServerData data, AppEnglish.EngServRef.PropertyData fil);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/GetSortedItems", ReplyAction="http://tempuri.org/IEngService/GetSortedItemsResponse")]
        int[] GetSortedItems(AppEnglish.EngServRef.ServerData data, AppEnglish.EngServRef.PropertyData property, bool desc);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/GetSortedItems", ReplyAction="http://tempuri.org/IEngService/GetSortedItemsResponse")]
        System.Threading.Tasks.Task<int[]> GetSortedItemsAsync(AppEnglish.EngServRef.ServerData data, AppEnglish.EngServRef.PropertyData property, bool desc);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/GetUserItemWords", ReplyAction="http://tempuri.org/IEngService/GetUserItemWordsResponse")]
        int[] GetUserItemWords(int user, int item, AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/GetUserItemWords", ReplyAction="http://tempuri.org/IEngService/GetUserItemWordsResponse")]
        System.Threading.Tasks.Task<int[]> GetUserItemWordsAsync(int user, int item, AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/GetItemData", ReplyAction="http://tempuri.org/IEngService/GetItemDataResponse")]
        int[] GetItemData(int id, AppEnglish.EngServRef.ServerData data, AppEnglish.EngServRef.ServerData res);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/GetItemData", ReplyAction="http://tempuri.org/IEngService/GetItemDataResponse")]
        System.Threading.Tasks.Task<int[]> GetItemDataAsync(int id, AppEnglish.EngServRef.ServerData data, AppEnglish.EngServRef.ServerData res);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/GetUserId", ReplyAction="http://tempuri.org/IEngService/GetUserIdResponse")]
        System.Nullable<int> GetUserId(string login);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/GetUserId", ReplyAction="http://tempuri.org/IEngService/GetUserIdResponse")]
        System.Threading.Tasks.Task<System.Nullable<int>> GetUserIdAsync(string login);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/CheckExistence", ReplyAction="http://tempuri.org/IEngService/CheckExistenceResponse")]
        bool CheckExistence(string name, AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/CheckExistence", ReplyAction="http://tempuri.org/IEngService/CheckExistenceResponse")]
        System.Threading.Tasks.Task<bool> CheckExistenceAsync(string name, AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/CheckAuthor", ReplyAction="http://tempuri.org/IEngService/CheckAuthorResponse")]
        bool CheckAuthor(string name, string surname);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/CheckAuthor", ReplyAction="http://tempuri.org/IEngService/CheckAuthorResponse")]
        System.Threading.Tasks.Task<bool> CheckAuthorAsync(string name, string surname);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/CheckUserPswd", ReplyAction="http://tempuri.org/IEngService/CheckUserPswdResponse")]
        bool CheckUserPswd(string login, string pswd);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/CheckUserPswd", ReplyAction="http://tempuri.org/IEngService/CheckUserPswdResponse")]
        System.Threading.Tasks.Task<bool> CheckUserPswdAsync(string login, string pswd);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/CheckAbsolute", ReplyAction="http://tempuri.org/IEngService/CheckAbsoluteResponse")]
        System.Nullable<bool> CheckAbsolute(int id, AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/CheckAbsolute", ReplyAction="http://tempuri.org/IEngService/CheckAbsoluteResponse")]
        System.Threading.Tasks.Task<System.Nullable<bool>> CheckAbsoluteAsync(int id, AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/RemoveItem", ReplyAction="http://tempuri.org/IEngService/RemoveItemResponse")]
        void RemoveItem(int id, AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/RemoveItem", ReplyAction="http://tempuri.org/IEngService/RemoveItemResponse")]
        System.Threading.Tasks.Task RemoveItemAsync(int id, AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/RemoveFullItemData", ReplyAction="http://tempuri.org/IEngService/RemoveFullItemDataResponse")]
        void RemoveFullItemData(int id, AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/RemoveFullItemData", ReplyAction="http://tempuri.org/IEngService/RemoveFullItemDataResponse")]
        System.Threading.Tasks.Task RemoveFullItemDataAsync(int id, AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/RemoveItemWord", ReplyAction="http://tempuri.org/IEngService/RemoveItemWordResponse")]
        void RemoveItemWord(int item, int word, AppEnglish.EngServRef.ServerData data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEngService/RemoveItemWord", ReplyAction="http://tempuri.org/IEngService/RemoveItemWordResponse")]
        System.Threading.Tasks.Task RemoveItemWordAsync(int item, int word, AppEnglish.EngServRef.ServerData data);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IEngServiceChannel : AppEnglish.EngServRef.IEngService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class EngServiceClient : System.ServiceModel.ClientBase<AppEnglish.EngServRef.IEngService>, AppEnglish.EngServRef.IEngService {
        
        public EngServiceClient() {
        }
        
        public EngServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public EngServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public EngServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public EngServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Nullable<int> AddUser(string login, string pswd, string avatar, string role, int level) {
            return base.Channel.AddUser(login, pswd, avatar, role, level);
        }
        
        public System.Threading.Tasks.Task<System.Nullable<int>> AddUserAsync(string login, string pswd, string avatar, string role, int level) {
            return base.Channel.AddUserAsync(login, pswd, avatar, role, level);
        }
        
        public System.Nullable<int> AddVideo(string name, string desc, string path, string sub, string img, bool absolute, System.Nullable<int> mark, System.Nullable<int> user, System.Nullable<int> year, System.DateTime created) {
            return base.Channel.AddVideo(name, desc, path, sub, img, absolute, mark, user, year, created);
        }
        
        public System.Threading.Tasks.Task<System.Nullable<int>> AddVideoAsync(string name, string desc, string path, string sub, string img, bool absolute, System.Nullable<int> mark, System.Nullable<int> user, System.Nullable<int> year, System.DateTime created) {
            return base.Channel.AddVideoAsync(name, desc, path, sub, img, absolute, mark, user, year, created);
        }
        
        public System.Nullable<int> AddBook(string name, string desc, string path, string img, bool absolute, System.Nullable<int> mark, System.Nullable<int> user, System.Nullable<int> year, System.DateTime created) {
            return base.Channel.AddBook(name, desc, path, img, absolute, mark, user, year, created);
        }
        
        public System.Threading.Tasks.Task<System.Nullable<int>> AddBookAsync(string name, string desc, string path, string img, bool absolute, System.Nullable<int> mark, System.Nullable<int> user, System.Nullable<int> year, System.DateTime created) {
            return base.Channel.AddBookAsync(name, desc, path, img, absolute, mark, user, year, created);
        }
        
        public System.Nullable<int> AddAuthor(string name, string surname) {
            return base.Channel.AddAuthor(name, surname);
        }
        
        public System.Threading.Tasks.Task<System.Nullable<int>> AddAuthorAsync(string name, string surname) {
            return base.Channel.AddAuthorAsync(name, surname);
        }
        
        public System.Nullable<int> AddCategory(string name, AppEnglish.EngServRef.ServerData data) {
            return base.Channel.AddCategory(name, data);
        }
        
        public System.Threading.Tasks.Task<System.Nullable<int>> AddCategoryAsync(string name, AppEnglish.EngServRef.ServerData data) {
            return base.Channel.AddCategoryAsync(name, data);
        }
        
        public void AddItemCategory(int item, int cat, AppEnglish.EngServRef.ServerData data) {
            base.Channel.AddItemCategory(item, cat, data);
        }
        
        public System.Threading.Tasks.Task AddItemCategoryAsync(int item, int cat, AppEnglish.EngServRef.ServerData data) {
            return base.Channel.AddItemCategoryAsync(item, cat, data);
        }
        
        public void AddBookAuthor(int bookId, int author) {
            base.Channel.AddBookAuthor(bookId, author);
        }
        
        public System.Threading.Tasks.Task AddBookAuthorAsync(int bookId, int author) {
            return base.Channel.AddBookAuthorAsync(bookId, author);
        }
        
        public void EditData(int id, string changes, AppEnglish.EngServRef.ServerData data, AppEnglish.EngServRef.PropertyData property) {
            base.Channel.EditData(id, changes, data, property);
        }
        
        public System.Threading.Tasks.Task EditDataAsync(int id, string changes, AppEnglish.EngServRef.ServerData data, AppEnglish.EngServRef.PropertyData property) {
            return base.Channel.EditDataAsync(id, changes, data, property);
        }
        
        public void EditMark(int id, int usersId, int changes, AppEnglish.EngServRef.ServerData data) {
            base.Channel.EditMark(id, usersId, changes, data);
        }
        
        public System.Threading.Tasks.Task EditMarkAsync(int id, int usersId, int changes, AppEnglish.EngServRef.ServerData data) {
            return base.Channel.EditMarkAsync(id, usersId, changes, data);
        }
        
        public bool Upload(byte[] file, string name, AppEnglish.EngServRef.FilesType type) {
            return base.Channel.Upload(file, name, type);
        }
        
        public System.Threading.Tasks.Task<bool> UploadAsync(byte[] file, string name, AppEnglish.EngServRef.FilesType type) {
            return base.Channel.UploadAsync(file, name, type);
        }
        
        public byte[] Download(string name, AppEnglish.EngServRef.FilesType type) {
            return base.Channel.Download(name, type);
        }
        
        public System.Threading.Tasks.Task<byte[]> DownloadAsync(string name, AppEnglish.EngServRef.FilesType type) {
            return base.Channel.DownloadAsync(name, type);
        }
        
        public void Delete(string name, AppEnglish.EngServRef.FilesType type) {
            base.Channel.Delete(name, type);
        }
        
        public System.Threading.Tasks.Task DeleteAsync(string name, AppEnglish.EngServRef.FilesType type) {
            return base.Channel.DeleteAsync(name, type);
        }
        
        public string GetItemProperty(int id, AppEnglish.EngServRef.ServerData data, AppEnglish.EngServRef.PropertyData property) {
            return base.Channel.GetItemProperty(id, data, property);
        }
        
        public System.Threading.Tasks.Task<string> GetItemPropertyAsync(int id, AppEnglish.EngServRef.ServerData data, AppEnglish.EngServRef.PropertyData property) {
            return base.Channel.GetItemPropertyAsync(id, data, property);
        }
        
        public System.Nullable<int> GetMark(int itemId, int userId, AppEnglish.EngServRef.ServerData data) {
            return base.Channel.GetMark(itemId, userId, data);
        }
        
        public System.Threading.Tasks.Task<System.Nullable<int>> GetMarkAsync(int itemId, int userId, AppEnglish.EngServRef.ServerData data) {
            return base.Channel.GetMarkAsync(itemId, userId, data);
        }
        
        public int[] GetItems(AppEnglish.EngServRef.ServerData data) {
            return base.Channel.GetItems(data);
        }
        
        public System.Threading.Tasks.Task<int[]> GetItemsAsync(AppEnglish.EngServRef.ServerData data) {
            return base.Channel.GetItemsAsync(data);
        }
        
        public int[] GetFItems(string filter, AppEnglish.EngServRef.ServerData data, AppEnglish.EngServRef.PropertyData fil) {
            return base.Channel.GetFItems(filter, data, fil);
        }
        
        public System.Threading.Tasks.Task<int[]> GetFItemsAsync(string filter, AppEnglish.EngServRef.ServerData data, AppEnglish.EngServRef.PropertyData fil) {
            return base.Channel.GetFItemsAsync(filter, data, fil);
        }
        
        public int[] GetSortedItems(AppEnglish.EngServRef.ServerData data, AppEnglish.EngServRef.PropertyData property, bool desc) {
            return base.Channel.GetSortedItems(data, property, desc);
        }
        
        public System.Threading.Tasks.Task<int[]> GetSortedItemsAsync(AppEnglish.EngServRef.ServerData data, AppEnglish.EngServRef.PropertyData property, bool desc) {
            return base.Channel.GetSortedItemsAsync(data, property, desc);
        }
        
        public int[] GetUserItemWords(int user, int item, AppEnglish.EngServRef.ServerData data) {
            return base.Channel.GetUserItemWords(user, item, data);
        }
        
        public System.Threading.Tasks.Task<int[]> GetUserItemWordsAsync(int user, int item, AppEnglish.EngServRef.ServerData data) {
            return base.Channel.GetUserItemWordsAsync(user, item, data);
        }
        
        public int[] GetItemData(int id, AppEnglish.EngServRef.ServerData data, AppEnglish.EngServRef.ServerData res) {
            return base.Channel.GetItemData(id, data, res);
        }
        
        public System.Threading.Tasks.Task<int[]> GetItemDataAsync(int id, AppEnglish.EngServRef.ServerData data, AppEnglish.EngServRef.ServerData res) {
            return base.Channel.GetItemDataAsync(id, data, res);
        }
        
        public System.Nullable<int> GetUserId(string login) {
            return base.Channel.GetUserId(login);
        }
        
        public System.Threading.Tasks.Task<System.Nullable<int>> GetUserIdAsync(string login) {
            return base.Channel.GetUserIdAsync(login);
        }
        
        public bool CheckExistence(string name, AppEnglish.EngServRef.ServerData data) {
            return base.Channel.CheckExistence(name, data);
        }
        
        public System.Threading.Tasks.Task<bool> CheckExistenceAsync(string name, AppEnglish.EngServRef.ServerData data) {
            return base.Channel.CheckExistenceAsync(name, data);
        }
        
        public bool CheckAuthor(string name, string surname) {
            return base.Channel.CheckAuthor(name, surname);
        }
        
        public System.Threading.Tasks.Task<bool> CheckAuthorAsync(string name, string surname) {
            return base.Channel.CheckAuthorAsync(name, surname);
        }
        
        public bool CheckUserPswd(string login, string pswd) {
            return base.Channel.CheckUserPswd(login, pswd);
        }
        
        public System.Threading.Tasks.Task<bool> CheckUserPswdAsync(string login, string pswd) {
            return base.Channel.CheckUserPswdAsync(login, pswd);
        }
        
        public System.Nullable<bool> CheckAbsolute(int id, AppEnglish.EngServRef.ServerData data) {
            return base.Channel.CheckAbsolute(id, data);
        }
        
        public System.Threading.Tasks.Task<System.Nullable<bool>> CheckAbsoluteAsync(int id, AppEnglish.EngServRef.ServerData data) {
            return base.Channel.CheckAbsoluteAsync(id, data);
        }
        
        public void RemoveItem(int id, AppEnglish.EngServRef.ServerData data) {
            base.Channel.RemoveItem(id, data);
        }
        
        public System.Threading.Tasks.Task RemoveItemAsync(int id, AppEnglish.EngServRef.ServerData data) {
            return base.Channel.RemoveItemAsync(id, data);
        }
        
        public void RemoveFullItemData(int id, AppEnglish.EngServRef.ServerData data) {
            base.Channel.RemoveFullItemData(id, data);
        }
        
        public System.Threading.Tasks.Task RemoveFullItemDataAsync(int id, AppEnglish.EngServRef.ServerData data) {
            return base.Channel.RemoveFullItemDataAsync(id, data);
        }
        
        public void RemoveItemWord(int item, int word, AppEnglish.EngServRef.ServerData data) {
            base.Channel.RemoveItemWord(item, word, data);
        }
        
        public System.Threading.Tasks.Task RemoveItemWordAsync(int item, int word, AppEnglish.EngServRef.ServerData data) {
            return base.Channel.RemoveItemWordAsync(item, word, data);
        }
    }
}
