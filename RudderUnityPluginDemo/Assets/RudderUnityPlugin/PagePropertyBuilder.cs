using System.Collections.Generic;

class PagePropertyBuilder
{
    private string title;
    public PagePropertyBuilder SetTitle(string title)
    {
        this.title = title;
        return this;
    }

    private string url;
    public PagePropertyBuilder SetUrl(string url)
    {
        this.url = url;
        return this;
    }

    private string path;
    public PagePropertyBuilder SetPath(string path)
    {
        this.path = path;
        return this;
    }

    private string referrer;
    public PagePropertyBuilder SetReferrer(string referrer)
    {
        this.referrer = referrer;
        return this;
    }

    private string search;
    public PagePropertyBuilder SetSearch(string search)
    {
        this.search = search;
        return this;
    }

    private string keywords;
    public PagePropertyBuilder SetKeywords(string keywords)
    {
        this.keywords = keywords;
        return this;
    }

    public Dictionary<string, object> Build()
    {
        if (url == null)
        {
            throw new RudderException("Key \"url\" is required for track event");
        }

        Dictionary<string, object> property = new Dictionary<string, object>();
        if (title != null)
        {
            property.Add("title", title);
        }
        property.Add("url", url);
        if (url != null)
        {
            property.Add("path", path);
        }
        if (referrer != null)
        {
            property.Add("referrer", referrer);
        }
        if (search != null)
        {
            property.Add("search", search);
        }
        if (keywords != null)
        {
            property.Add("keywords", keywords);
        }
        return property;
    }
}