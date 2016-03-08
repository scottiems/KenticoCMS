<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Chat_Controls_UI_ChatRoom_Edit" CodeFile="Edit.ascx.cs" %>

<cms:UIForm runat="server" ID="EditForm" ObjectType="chat.room" RedirectUrlAfterCreate="Frameset.aspx?roomid={%EditedObject.ID%}&siteid={?siteid?}&saved=1" />
