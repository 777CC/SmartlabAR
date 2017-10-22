using System.Collections.Generic;
[System.Serializable]
public class Product{
    public string Header;
    public string Desc;
}
[System.Serializable]
public class Info
{
    public Product[] ProductList;
}