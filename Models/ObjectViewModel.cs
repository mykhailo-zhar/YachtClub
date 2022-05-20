using Project.Migrations;
using System.Collections.Generic;
using System.Linq;

namespace Project.Models
{
    public class CRUD
    {
        public static readonly string Create = "Create";
        public static readonly string Edit = "Edit";
        public static readonly string Details = "Details";
        public static readonly string Delete = "Delete";
    }
    public class ObjectViewModel<T>
    {
        public T Object { get; set; }
        public bool[] Option { get; set; } = new bool[4];
        public bool ReadOnly { get; set; } = false;
        public bool ShowAction { get; set; } = true;
        public bool ShowReadOnlyFields { get; set; } = true;
        public string Action { get; set; } = CRUD.Create;
        public string ReturnUrl { get; set; } = "";
        public string Theme { get; set; } = "primary";

        public string ActionObjectName => Action + Object.GetType().Name;

        public string DisplayNone(bool Condition = false,params string[] actions) => actions.Contains(Action) || Condition? "d-none" : "";

        public string HideOnCreate => DisplayNone(actions: CRUD.Create);
        public string HideOnDelete => DisplayNone(actions: CRUD.Delete);
        public string HideOnEdit => DisplayNone(actions: CRUD.Edit);
        public string AlwaysHidden => DisplayNone(Condition: true);
        
    }

    public class ObjectViewModelFactory<T>
    {
        public static ObjectViewModel<T> Edit(T obj, string Returnurl = "")
        {
            return new ObjectViewModel<T>
            {
                Object = obj,
                Action = CRUD.Edit,
                ReturnUrl = Returnurl
            };

        }
        public static ObjectViewModel<T> Create(T obj, string Returnurl = "")
        {
            return new ObjectViewModel<T>
            {
                Object = obj,
                ShowReadOnlyFields = false,
                ReturnUrl = Returnurl
            };

        }
        public static ObjectViewModel<T> Delete(T obj, string Returnurl = "")
        {
            return new ObjectViewModel<T>
            {
                Object = obj,
                Action = CRUD.Delete,
                Theme = "danger",
                ShowReadOnlyFields = true,
                ReadOnly = true,
                ReturnUrl = Returnurl
            };

        }
        public static ObjectViewModel<T> Details(T obj, string Returnurl = "")
        {
            return new ObjectViewModel<T>
            {
                Object = obj,
                Action = CRUD.Details,
                ReadOnly = true,
                Theme = "info",
                ShowAction = false,
                ReturnUrl = Returnurl
            };
        }


    }

    /*
     
      <div class="form-group">
        <label asp-for="Product.CategoryId">Category</label>
        <div>
            <span asp-validation-for="Product.CategoryId" class="text-danger"></span>
        </div>
        <select asp-for="Product.CategoryId" class="form-control"
                disabled="@Model.ReadOnly"
                asp-items="@(new SelectList(Model.Categories,
"CategoryId", "Name"))">
            <option value="" disabled selected>Choose a Category</option>
        </select>
    </div>
    <div class="form-group">
        <label asp-for="Product.SupplierId">Supplier</label>
        <div>
            <span asp-validation-for="Product.SupplierId" class="text-danger"></span>
        </div>
        <select asp-for="Product.SupplierId" class="form-control"
                disabled="@Model.ReadOnly"
                asp-items="@(new SelectList(Model.Suppliers,
"SupplierId", "Name"))">
            <option value="" disabled selected>Choose a Supplier</option>
        </select>
    </div>
     */
}
