using ToSic.Eav.Data;
using ToSic.Eav.DataSource;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using System;
using System.Collections.Generic;
using System.Linq;

// 2sxclint:disable:no-EntityTitle-in-quotes
// 2sxclint:disable:avoid-dynamic

namespace AppCode.Source
{
  // Class to generate shared parts on the page
  // Such as navigations etc.
  // Should itself not have much code, it's more central API to access everything
  public class ViewConfigurationSimulation: Custom.Hybrid.CodeTyped
  {
    #region Constructor and Main Objects

    private const string ViewConfigCode = "ViewConfig";

    public ViewConfigurationSimulation Setup(dynamic sys, dynamic tabHandler) {
      TabHandler = tabHandler;
      Fancybox = sys.Fancybox;
      return this;
    }

    internal dynamic TabHandler;
    private dynamic Fancybox;

    #endregion

    #region Main Infos and Generate ViewConfig-Tab

    public string ContentType;
    public IEntity DemoItem;
    public List<IEntity> ContentList;
    public string PresentationType;
    public List<IEntity> PresentationList;
    public string HeaderType;
    public IEntity HeaderItem;
    public bool IsList;
    public string QueryName;

    public IHtmlTag TabContents() {
      var t = Kit.HtmlTags;
      var cList = t.Ul();
      if (ContentType.Has()) cList = cList.Add(t.Li("Content/Item ContentType: ", t.Strong(ContentType)));
      if (DemoItem != null) cList = cList.Add(t.Li("Content/Item Demo-Data: ", t.Strong(DemoItem.Get("EntityTitle")), " (ID: " + DemoItem.EntityId + ")"));

      // List
      if (IsList) cList = cList.Add(t.Li("Content/Item IsList: ", t.Strong(IsList.ToString())));
      if (ContentList != null) cList = cList.Add(
        t.Li(
          "Content/Item Data: ",
          t.Ol(
            ContentList.Select((ent, i) => t.Li(
              t.Strong(ent.Get("EntityTitle")),
              " (ID: " + ent.EntityId + ")",
              PresentationList != null && PresentationList.Count() > 0
                ? " - Presentation: " + PresentationList.ElementAt(i).Get("EntityTitle") + " (ID: " + PresentationList.ElementAt(i).EntityId + ")"
                : null
            ))
          )
        )
      );

      // Header
      if (HeaderType.Has()) cList = cList.Add(t.Li("Header Type: ", t.Strong(HeaderType)));
      if (HeaderItem != null) cList = cList.Add(t.Li("Header Item: ", t.Strong(HeaderItem.Get("EntityTitle")), " (ID: " + HeaderItem.EntityId + ")"));

      // Query - with details and images
      var queryDetails = t.RawHtml();
      if (QueryName.Has()) {
        cList = cList.Add(t.Li("Query: ", t.Strong(QueryName)));

        var qInfoKey = "query-" + QueryName.ToLowerInvariant();
        var qInfoEntity = App.Data["TutorialObjectInfo"].List.FirstOrDefault(i => i.Get<string>("NameId").ToLowerInvariant() == qInfoKey);
        if (qInfoEntity != null) {
          var qInfo = AsItem(qInfoEntity);
          queryDetails = queryDetails.Add(
            t.H5("Details for " + qInfo.String("Title")).Attr(Kit.Toolbar.Empty().Edit(qInfo)),
            qInfo.String("Description"),
            Fancybox.Gallery(qInfo, "Images")
          );
        } else {
          // add toolbar if no info yet
          queryDetails = queryDetails.Add(t.P("No details yet for " + QueryName).Attr(Kit.Toolbar.Empty().New("TutorialObjectInfo", prefill: new { NameId = qInfoKey })));
        }
      }

      return t.RawHtml(cList, queryDetails);
    }

    #endregion

    #region Public Helpers to Simulate View Content

    /// <summary>
    /// Helper to get either the query or the type
    /// </summary>
    private IEnumerable<IEntity> GetListForSimulate(string type = null, string nameId = null, string query = null, string stream = null, object parameters = null) {
      var l = Log.Call<IEnumerable<IEntity>>("type: " + type + "; nameId: " + nameId + "; query: " + query + "; stream: " + stream);
      // Prepare: Verify the Tab "ViewConfig" was specified
      // throw new Exception("th:" + TabHandler);
      var th = TabHandler as dynamic;
      if (th.Tabs == null || !th.Tabs.ContainsKey(ViewConfigCode))
        throw new Exception("Tab '" + ViewConfigCode + "' not found - make sure the view has this");

      IEnumerable<IEntity> data = null;

      // Case 1: Get Content-Type
      if (type != null) {
        data = App.Data[type].List;
        if (!data.Any()) throw new Exception("Trying to simulate view content - but type returned no data");
      }

      // Case 2: Get a query, possibly a stream
      if (query != null) {
        var q = App.GetQuery(query, parameters: parameters);
        data = q.GetStream(stream).List; // should work for both null and "some-name"
        if (!data.Any()) throw new Exception("Trying to simulate view content - but query returned no data");
      }

      if (nameId != null) {
        var ent = data.First(e => e.Get<string>("NameId") == nameId);
        data = new List<IEntity> { ent };
      }

      // TODO: get by nameid
      return l(data, "ok");
    }

    public IEntity Content(string type = null, string nameId = null, string query = null, string stream = null, object parameters = null) {
      var l = Log.Call<IEntity>();
      var list = GetListForSimulate(type, nameId, query, stream, parameters: parameters);
      var first = list.First();
      ContentType = first.Type.Name;
      ContentList = list.Take(1).ToList();
      return l(first, "ok");
    }

    public IEntity Header(string type = null, string nameId = null, string query = null, string stream = null) {
      var l = Log.Call<IEntity>();
      var list = GetListForSimulate(type, nameId, query, stream);
      var first = list.First();
      HeaderType = first.Type.Name;
      HeaderItem = first;
      return l(first, "ok");
    }

    public IEnumerable<IEntity> List(string type = null, string nameId = null, string query = null, string stream = null) {
      var l = Log.Call<IEnumerable<IEntity>>();

      // Prepare: Set common ViewConfig props
      IsList = true;
      
      var list = GetListForSimulate(type, nameId, query, stream);
      ContentType = list.First().Type.Name;
      ContentList = list.ToList();
      return l(list, "ok");
    }
    public IDataSource Query(string query = null, string stream = null, object parameters = null) {
      var l = Log.Call<IDataSource>();

      // Prepare: Verify the Tab "ViewConfig" was specified
      if (TabHandler.Tabs == null || !TabHandler.Tabs.ContainsKey(ViewConfigCode))
        throw new Exception("Tab '" + ViewConfigCode + "' not found - make sure the view has this");

      var q = App.GetQuery(query, parameters: parameters);
      QueryName = query;
      return l(q, "ok");
    }

    public IEnumerable<IEntity> PresList(IEnumerable<IEntity> myItems, bool padWithNull = true, string type = null, string nameId = null, string query = null, string stream = null) {
      var l = Log.Call<IEnumerable<IEntity>>();

      var list = GetListForSimulate(type, nameId, query, stream);
      PresentationType = list.First().Type.Name;

      if (list.Count() < myItems.Count()) {
        var pad = padWithNull ? (IEntity)null : list.First();
        list = list.Concat(Enumerable.Repeat(pad, myItems.Count() - list.Count()));
      }
      PresentationList = list.ToList();
      return l(list, "ok");
    }

    #endregion

  }
}