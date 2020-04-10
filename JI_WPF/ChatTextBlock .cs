using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace JI_WPF
{
    public class ChatTextBlock : TextBlock
    {
        public InlineCollection InlineCollection
        {
            get
            {
                return (InlineCollection)GetValue(InlineCollectionProperty);
            }
            set
            {
                SetValue(InlineCollectionProperty, value);
            }
        }

        public static readonly DependencyProperty InlineCollectionProperty = DependencyProperty.Register(
            "InlineCollection",
            typeof(InlineCollection),
            typeof(ChatTextBlock),
                new UIPropertyMetadata((PropertyChangedCallback)((sender, args) =>
                {
                    ChatTextBlock textBlock = sender as ChatTextBlock;

                    if (textBlock != null)
                    {
                        textBlock.Inlines.Clear();

                        InlineCollection inlines = args.NewValue as InlineCollection;

                        if (inlines != null)
                            textBlock.Inlines.AddRange(inlines.ToList());
                    }
                })));
    }
}
