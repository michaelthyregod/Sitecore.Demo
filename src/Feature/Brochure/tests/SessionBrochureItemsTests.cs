﻿namespace Sitecore.Feature.Brochure.Tests
{
  using System;
  using System.Collections.Generic;
  using System.Web;
  using FluentAssertions;
  using Sitecore.Data;
  using Sitecore.FakeDb;
  using Sitecore.Feature.Brochure.Models;
  using Sitecore.Foundation.Testing.Attributes;
  using Xunit;

  public class SessionBrochureItemsTests
  {
    [AutoDbData]
    [Theory]
    public void SessionBrochureItems_EmptySession_ItemsNotNullAndEmpty(HttpContext context)
    {
      var brochureItems = new SessionBrochureItems(context);
      brochureItems.Items.Should().NotBeNull();
      brochureItems.Items.Should().BeEmpty();
    }

    [AutoDbData]
    [Theory]
    public void SessionBrochureItems_InitialisedSession_ItemsReturnsListOfBrochureItems(HttpContext context, List<BrochureItem> itemList)
    {
      context.Session.Add(SessionBrochureItems.SessionKey, itemList);
      var brochureItems = new SessionBrochureItems(context);

      brochureItems.Items.ShouldBeEquivalentTo(itemList);
    }

    [AutoDbData]
    [Theory]
    public void Add_ItemAlreadyExists_ReturnsFalse(Db database, HttpContext context, List<BrochureItem> itemList)
    {
      database.Add(new DbItem("existing item", itemList[0].ItemID));
      var item = database.GetItem(itemList[0].ItemID);

      context.Session.Add(SessionBrochureItems.SessionKey, itemList);
      var brochureItems = new SessionBrochureItems(context);
      brochureItems.Add(item).Should().BeFalse();
    }

    [AutoDbData]
    [Theory]
    public void Add_ItemDoesNotExist_ReturnsTrueAndItemsContainItem(Db database, HttpContext context, List<BrochureItem> itemList)
    {
      var itemID = ID.NewID;
      database.Add(new DbItem("existing item", itemID));
      var item = database.GetItem(itemID);

      context.Session.Add(SessionBrochureItems.SessionKey, itemList);
      var brochureItems = new SessionBrochureItems(context);
      brochureItems.Add(item).Should().BeTrue();
      brochureItems.Items.Should().Contain(f => f.ItemID == itemID);
    }

    [AutoDbData]
    [Theory]
    public void Remove_ItemNotInItems_ReturnsFalse(Db database, HttpContext context, List<BrochureItem> itemList)
    {
      var itemID = ID.NewID;
      database.Add(new DbItem("new item", itemID));
      var item = database.GetItem(itemID);

      context.Session.Add(SessionBrochureItems.SessionKey, itemList);
      var brochureItems = new SessionBrochureItems(context);
      brochureItems.Remove(item).Should().BeFalse();
    }

    [AutoDbData]
    [Theory]
    public void Remove_ItemInItems_ReturnsTrueAndItemsNotContainItem(Db database, HttpContext context, List<BrochureItem> itemList)
    {
      var itemID = itemList[0].ItemID;
      database.Add(new DbItem("existing item", itemID));
      var item = database.GetItem(itemID);

      context.Session.Add(SessionBrochureItems.SessionKey, itemList);
      var brochureItems = new SessionBrochureItems(context);
      brochureItems.Remove(item).Should().BeTrue();
      brochureItems.Items.Should().NotContain(f => f.ItemID == itemID);
    }

    [AutoDbData]
    [Theory]
    public void Remove_ItemIsNull_ThrowsException(HttpContext context)
    {
      var brochureItems = new SessionBrochureItems(context);
      Assert.Throws<ArgumentNullException>(() => brochureItems.Remove(null));
    }

    [AutoDbData]
    [Theory]
    public void Add_ItemIsNull_ThrowsException(HttpContext context)
    {
      var brochureItems = new SessionBrochureItems(context);
      Assert.Throws<ArgumentNullException>(() => brochureItems.Add(null));
    }
  }
}