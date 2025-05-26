using CommunityToolkit.Mvvm.ComponentModel;
using FyreWorksPM.DataAccess.Data.Models;
using FyreWorksPM.DataAccess.DTO;

namespace FyreWorksPM.ViewModels.Creation
{
    public partial class BidMaterialLineItemViewModel : ObservableObject
    {
        private readonly CreateBidViewModel _parent;
        private bool unitSaleManuallySet;

        public BidMaterialLineItemViewModel(BidMaterialLineItemModel item, CreateBidViewModel parent, Action raiseTotalsCallback)
        {
            Item = item;
            _parent = parent;
            RaiseTotalsCallback = raiseTotalsCallback;
        }

        public Action? RaiseTotalsCallback { get; set; }

        private ItemDto? selectedItem;
        public ItemDto? SelectedItem
        {
            get => selectedItem;
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;

                    if (value != null)
                    {
                        ItemName = value.Name;
                        Description = value.Description;
                        ItemId = value.Id;
                        // optionally UnitCost and UnitSale, if you ever save them from the item
                    }

                    OnPropertyChanged();
                }
            }
        }



        public BidMaterialLineItemModel Item { get; }

        public int? ItemId
        {
            get => Item.ItemId;
            set
            {
                if (Item.ItemId != value)
                {
                    Item.ItemId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ItemName
        {
            get => Item.ItemName;
            set
            {
                if (Item.ItemName != value)
                {
                    Item.ItemName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => Item.Description;
            set
            {
                if (Item.Description != value)
                {
                    Item.Description = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal Qty
        {
            get => Item.Qty;
            set
            {
                if (Item.Qty != value)
                {
                    Item.Qty = value;
                    OnPropertyChanged();
                    RaiseTotalsCallback?.Invoke();

                }
            }
        }

        public decimal UnitCost
        {
            get => Item.UnitCost;
            set
            {
                if (Item.UnitCost != value)
                {
                    Item.UnitCost = value;
                    unitSaleManuallySet = false;

                    Item.UnitSale = Math.Round(Item.UnitCost * (1 + _parent.MaterialMarkup / 100), 2);
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UnitSale));
                    OnPropertyChanged(nameof(IsSaleOverridden));
                    RaiseTotalsCallback?.Invoke();

                }
            }
        }

        public decimal UnitSale
        {
            get => Item.UnitSale;
            set
            {
                if (Item.UnitSale != value)
                {
                    Item.UnitSale = value;
                    unitSaleManuallySet = true;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsSaleOverridden));
                    RaiseTotalsCallback?.Invoke();
                }
            }
        }

        public bool IsSaleOverridden =>
            Math.Round(UnitSale, 2) != Math.Round(UnitCost * (1 + _parent.MaterialMarkup / 100), 2);
    }

}
