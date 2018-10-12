﻿using System;
using CTP.Serialization;
using GSF.Collections;

namespace CTP
{
    public abstract class DocumentObject
    {
        internal DocumentObject()
        {

        }

        public abstract CtpDocument ToDocument();

        public static implicit operator CtpDocument(DocumentObject obj)
        {
            return obj.ToDocument();
        }

        public override string ToString()
        {
            return ToDocument().ToYAML();
        }

        public virtual void BeforeLoad()
        {

        }

        public virtual void AfterLoad()
        {

        }

        internal virtual void MissingValue(string name, CtpObject value)
        {
            throw new NotSupportedException();
        }

        internal virtual void MissingElement(string name)
        {
            throw new NotSupportedException();
        }
    }

    public abstract class DocumentObject<T>
        : DocumentObject
        where T : DocumentObject<T>
    {
        private static readonly CtpDocumentName CommandName;
        private static readonly Exception LoadError;
        private static readonly TypeSerializationMethodBase<T> Serialization;

        static DocumentObject()
        {
            LoadError = TypeSerialization<T>.LoadError;
            if (LoadError == null)
            {
                Serialization = TypeSerialization<T>.Serialization;
                CommandName = CtpDocumentName.Create(TypeSerialization<T>.CommandAttribute?.DocumentName ?? nameof(T));
            }
        }

        protected DocumentObject()
        {
            if (LoadError != null)
                throw LoadError;
            if (typeof(T) != GetType())
                throw new ArgumentException("The supplied type must exactly match the generic type parameter");
        }

        public override CtpDocument ToDocument()
        {
            return Save((T)this);
        }

        public static T FromDocument(CtpDocument obj)
        {
            return Load(obj);
        }

        public static CtpDocument Save(T obj)
        {
            if (LoadError != null)
                throw LoadError;
            var wr = new CtpDocumentWriter();
            wr.Initialize(CommandName);
            Serialization.Save(obj, wr, null);
            return wr.ToCtpDocument();
        }

        public static T Load(CtpDocument document)
        {
            if (LoadError != null)
                throw LoadError;
            if (CommandName.Value != document.RootElement)
                throw new Exception("Document Mismatch");
            var rdr = document.MakeReader();
            rdr.Read();
            return Serialization.Load(rdr);
        }

    }

}
