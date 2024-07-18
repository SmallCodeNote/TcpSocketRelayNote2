using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UserControlPanelViewSet
{
    public interface IPanelChildUserControl
    {
        int ChildIndex { get; set; }
        Action<int> DeleteThis { get; set; }
        Action ControlContentsChanged { get; set; }

        int Top { get; set; }
        int Height { get; set; }

        void ParamSetFromString(string Line);

        IPanelChildUserControl Clone();
        IPanelChildUserControl New(string Line);

        Control.ControlCollection Controls { get; }

    }

    class UserControlPanelView
    {
        //===================
        // Constructor
        //===================
        public UserControlPanelView(Panel panelFrame, TextBox textBox, IPanelChildUserControl[] userControlForms)
        {
            this.panelFrame = panelFrame;

            panelView = new Panel();
            panelView.Location = new System.Drawing.Point(0, 0);
            panelView.Name = panelFrame.Name + "_View";
            panelView.Size = new System.Drawing.Size(panelFrame.Width - 10, 0);

            this.panelFrame.Controls.Add(panelView);

            this.userControlForms = new List<IPanelChildUserControl>();
            this.userControlForms.AddRange(userControlForms);

            this.textBox = textBox;
        }

        //===================
        // Member variable
        //===================
        public Panel panelFrame;
        private Panel panelView;
        public TextBox textBox;

        public Action ControlContentsChanged;

        /// <summary>
        /// CopySourceInstanceList
        /// </summary>
        List<IPanelChildUserControl> userControlForms;

        public Control.ControlCollection Controls { get { return panelView.Controls; } }

        //===================
        // Member function
        //===================
        public void TextBoxUpdate(string text)
        {
            if (textBox.InvokeRequired)
            {
                textBox.Invoke(((Action)(() => TextBoxUpdate(text))));
            }
            else
            {
                textBox.Text = text;
            }
        }

        public void AddChildUserControl(IPanelChildUserControl userControl)
        {
            userControl.DeleteThis = DeleteThis;
            userControl.Top = panelView.Height;
            userControl.ControlContentsChanged = this.ControlContentsChanged;

            panelView.Height += userControl.Height;
            userControl.ChildIndex = panelView.Controls.Count;

            this.panelView.Controls.Add((Control)userControl);
        }

        public void DeleteThis(int childIndex)
        {
            for (int i = 0; i < this.panelView.Controls.Count; i++)
            {
                if (((IPanelChildUserControl)this.panelView.Controls[i]).ChildIndex == childIndex)
                {
                    this.panelView.Controls.RemoveAt(i);
                    ChildIndexRefresh();
                    return;
                }
            }
        }

        public void ChildIndexRefresh()
        {
            panelView.Height = 0;

            for (int i = 0; i < this.panelView.Controls.Count; i++)
            {
                ((IPanelChildUserControl)this.panelView.Controls[i]).ChildIndex = i;
                ((IPanelChildUserControl)this.panelView.Controls[i]).Top = panelView.Height;
                panelView.Height += ((IPanelChildUserControl)this.panelView.Controls[i]).Height;
            }
        }

        public void ChildUserControlsCreateFromTextBox()
        {
            panelView.Controls.Clear();
            panelView.Height = 0;
            string[] Lines = textBox.Text.Replace("\r\n", "\n").Trim('\n').Split('\n');
            foreach (var Line in Lines)
            {
                string[] cols = Line.Split('\t');
                string className = cols[0];

                AddChildUserControl(CreateChildUserControl(Line));
            }
        }

        public void ChildUserControlsStoreToTextBox()
        {
            List<string> Lines = new List<string>();
            if (userControlForms.Count == 1)
            {
                foreach (IPanelChildUserControl item in panelView.Controls)
                {
                    Lines.Add(item.ToString());
                }
            }
            else
            {
                foreach (IPanelChildUserControl item in panelView.Controls)
                {
                    Lines.Add(item.GetType().Name + "\t" + item.ToString());
                }
            }

            TextBoxUpdate(string.Join("\r\n", Lines.ToArray()));
        }

        IPanelChildUserControl CreateChildUserControl(string parameterString)
        {
            if (userControlForms.Count == 1)
            {
                return userControlForms[0].New(parameterString);
            }
            else
            {
                List<string> cols = new List<string>(parameterString.Split('\t'));
                string className = cols[0];
                cols.RemoveAt(0);

                List<IPanelChildUserControl> master = userControlForms.Where(control => control.GetType().Name == className).ToList();

                if (master.Count > 0)
                {
                    string typeName = master[0].GetType().Name;
                    return master[0].New(string.Join("\t", cols.ToArray()));
                }
                else
                {
                    return userControlForms[0].Clone();
                }
            }
        }

    }
}
