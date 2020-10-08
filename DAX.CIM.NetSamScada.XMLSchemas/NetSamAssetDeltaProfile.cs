
namespace DAX.CIM.NetSamScada.Delta.Asset {
    using DAX.CIM.NetSamScada.Asset;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://visue.dk/asset_delta_2_0.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://visue.dk/asset_delta_2_0.xsd", IsNullable=false)]
    public partial class ChangeSet {
        
        private ObjectCreation[] objectCreationField;
        
        private ObjectModification[] objectModificationField;
        
        private ObjectDeletion[] objectDeletionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ObjectCreation", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ObjectCreation[] ObjectCreation {
            get {
                return this.objectCreationField;
            }
            set {
                this.objectCreationField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ObjectModification", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ObjectModification[] ObjectModification {
            get {
                return this.objectModificationField;
            }
            set {
                this.objectModificationField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ObjectDeletion", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ObjectDeletion[] ObjectDeletion {
            get {
                return this.objectDeletionField;
            }
            set {
                this.objectDeletionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://visue.dk/asset_delta_2_0.xsd")]
    public partial class ObjectCreation : DataSetMember {
        
        private IdentifiedObject objectField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public IdentifiedObject Object {
            get {
                return this.objectField;
            }
            set {
                this.objectField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ObjectReverseModification))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://visue.dk/asset_delta_2_0.xsd")]
    public partial class ObjectForwardModification {
        
        private PropertyModification[] propertyField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Property", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public PropertyModification[] Property {
            get {
                return this.propertyField;
            }
            set {
                this.propertyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://visue.dk/asset_delta_2_0.xsd")]
    public partial class PropertyModification {
        
        private object valueField;
        
        private string refField;
        
        private string nameField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public object Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Ref {
            get {
                return this.refField;
            }
            set {
                this.refField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://visue.dk/asset_delta_2_0.xsd")]
    public partial class ObjectReverseModification : ObjectForwardModification {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ObjectDeletion))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ObjectModification))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ObjectCreation))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://visue.dk/asset_delta_2_0.xsd")]
    public abstract partial class DataSetMember {
        
        private string referenceTypeField;
        
        private string refField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string referenceType {
            get {
                return this.referenceTypeField;
            }
            set {
                this.referenceTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string @ref {
            get {
                return this.refField;
            }
            set {
                this.refField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://visue.dk/asset_delta_2_0.xsd")]
    public partial class ObjectDeletion : DataSetMember {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://visue.dk/asset_delta_2_0.xsd")]
    public partial class ObjectModification : DataSetMember {
        
        private PropertyModification[] forwardChangeField;
        
        private ObjectReverseModification reverseChangeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("Property", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public PropertyModification[] ForwardChange {
            get {
                return this.forwardChangeField;
            }
            set {
                this.forwardChangeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ObjectReverseModification ReverseChange {
            get {
                return this.reverseChangeField;
            }
            set {
                this.reverseChangeField = value;
            }
        }
    }
}
