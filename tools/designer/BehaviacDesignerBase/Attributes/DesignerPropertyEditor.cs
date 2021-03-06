////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2009, Daniel Kollmann
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list of conditions
//   and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list of
//   conditions and the following disclaimer in the documentation and/or other materials provided
//   with the distribution.
//
// - Neither the name of Daniel Kollmann nor the names of its contributors may be used to endorse
//   or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY
// WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
////////////////////////////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// The above software in this distribution may have been modified by THL A29 Limited ("Tencent Modifications").
//
// All Tencent Modifications are Copyright (C) 2015 THL A29 Limited.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Behaviac.Design.Attributes
{
    public partial class DesignerPropertyEditor : UserControl
    {
        public DesignerPropertyEditor()
        {
            InitializeComponent();
        }

        protected object _object;
        public object SelectedObject
        {
            get { return _object; }
        }

        protected Nodes.Node _root;
        public void SetRootNode(Nodes.Node root)
        {
            _root = root;
        }

        protected Type _filterType = null;
        public Type FilterType
        {
            get { return _filterType; }
            set { _filterType = value; }
        }

        protected ValueTypes _valueType = ValueTypes.All;
        public ValueTypes ValueType
        {
            get { return _valueType; }
            set { _valueType = value; }
        }

        protected DesignerPropertyInfo _property;
        public virtual void SetProperty(DesignerPropertyInfo property, object obj)
        {
            _property = property;
            _object = obj;
        }
        public DesignerPropertyInfo GetProperty()
        {
            return _property;
        }

        protected DesignerArrayPropertyInfo _arrayProperty;
        public virtual void SetArrayProperty(DesignerArrayPropertyInfo arrayProperty, object obj)
        {
            _arrayProperty = arrayProperty;
            _object = obj;
        }

        protected DesignerStructPropertyInfo _structProperty;
        public virtual void SetStructProperty(DesignerStructPropertyInfo structProperty, object obj)
        {
            _structProperty = structProperty;
            _object = obj;
        }

        protected MethodDef.Param _param;
        public virtual void SetParameter(MethodDef.Param param, object obj)
        {
            _param = param;
            _object = obj;
        }

        protected VariableDef _variable;
        public virtual void SetVariable(VariableDef value, object obj)
        {
            _variable = value;
            _object = obj;
        }
        public VariableDef GetVariable()
        {
            return _variable;
        }

        protected ParInfo _par;
        public virtual void SetPar(ParInfo par, object obj)
        {
            _par = par;
            _object = obj;

            SetVariable(par.Variable, obj);
        }

        public Attributes.DesignerProperty Attribute
        {
            get
            {
                if (_param != null)
                    return _param.Attribute;

                return _property.Attribute;
            }
        }

        protected bool _valueWasAssigned = false;
        public void ValueWasAssigned()
        {
            _valueWasAssigned = true;
        }
        public void ValueWasnotAssigned()
        {
            _valueWasAssigned = false;
        }

        public virtual void ReadOnly()
        {
        }

        public virtual void Clear()
        {
        }

        public virtual void SetRange(double min, double max)
        {
        }

        public delegate void InvalidateProperty();
        public static event InvalidateProperty PropertyChanged;

        protected void RereshProperty(bool byForce, DesignerPropertyInfo property)
        {
            if (!byForce)
            {
                DesignerPropertyEnum enumAtt = property.Attribute as DesignerPropertyEnum;
                if (enumAtt != null && enumAtt.DependingProperty != "")
                {
                    byForce = true;
                }
            }

            if (byForce && DesignerPropertyEditor.PropertyChanged != null)
            {
                this.BeginInvoke(new MethodInvoker(DesignerPropertyEditor.PropertyChanged));
            }
        }

        public delegate void ValueChanged(object sender, DesignerPropertyInfo property);
        public event ValueChanged ValueWasChanged;
        protected void OnValueChanged(DesignerPropertyInfo property)
        {
            if (!_valueWasAssigned)
                return;

            Nodes.Node node = _object as Nodes.Node;

            if (node != null)
            {
                node.OnPropertyValueChanged(true);
            }
            else
            {
                Attachments.Attachment attach = _object as Attachments.Attachment;
                if (attach != null)
                    attach.OnPropertyValueChanged(true);
            }

            if (ValueWasChanged != null)
                ValueWasChanged(this, property);
        }

        public virtual string DisplayName
        {
            get { return (Attribute != null) ? Attribute.DisplayName : string.Empty; }
        }

        public virtual string Description
        {
            get { return (Attribute != null) ? Attribute.Description : string.Empty; }
        }

        public delegate void DescriptionChanged(string displayName, string description);
        public event DescriptionChanged DescriptionWasChanged;
        protected void OnDescriptionChanged(string displayName, string description)
        {
            if (DescriptionWasChanged != null)
                DescriptionWasChanged(displayName, description);
        }
    }
}
