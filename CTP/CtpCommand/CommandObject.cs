﻿using System;
using System.Dynamic;
using CTP.IO;
using GSF.Collections;

namespace CTP
{
    /// <summary>
    /// The base class for all Object that can be automatically serialized to/from a <see cref="CtpCommand"/>.
    /// </summary>
    public abstract class CommandObject
    {
        //Ensures that users cannot directly inherit from this class.
        internal CommandObject()
        {

        }

        /// <summary>
        /// Gets the Schema associated with this command;
        /// </summary>
        public abstract CtpCommandSchema Schema { get; }

        /// <summary>
        /// The name that is associated with this command record. If this is a nested object, this value will be ignored.
        /// </summary>
        public abstract string CommandName { get; }

        /// <summary>
        /// Converts this object into a <see cref="CtpCommand"/>
        /// </summary>
        /// <returns></returns>
        public abstract CtpCommand ToCommand();

        public abstract byte[] ToDataCommandPacket(int schemeRuntimeID);

        /// <summary>
        /// Implicitly converts into a <see cref="CtpCommand"/>.
        /// </summary>
        /// <param name="obj"></param>
        public static explicit operator CtpCommand(CommandObject obj)
        {
            return obj.ToCommand();
        }

        /// <summary>
        /// A default <see cref="ToString"/> implementation that shows the YAML representation of the command
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToCommand().ToYAML();
        }

        /// <summary>
        /// When creating a new object from <see cref="CtpCommand"/>, this method is called first to allow the coder
        /// to define default values.
        /// </summary>
        protected virtual void BeforeLoad()
        {

        }

        /// <summary>
        /// This occurs after a new object is loaded. This allows the coder to validate or finish the loading process.
        /// </summary>
        protected virtual void AfterLoad()
        {

        }

        /// <summary>
        /// Occurs during loading when a value is present in the <see cref="CtpCommand"/> but
        /// there is not a corresponding field.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected virtual void MissingValue(string name, CtpObject value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Occurs during loading when a element is present in the <see cref="CtpCommand"/> but
        /// there is not a corresponding field.
        /// </summary>
        /// <param name="name"></param>
        protected virtual void MissingElement(string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Internal Method Called during the loading process.
        /// </summary>
        internal void OnBeforeLoad()
        {
            BeforeLoad();
        }

        /// <summary>
        /// Internal Method Called during the loading process.
        /// </summary>
        internal void OnAfterLoad()
        {
            AfterLoad();
        }

        /// <summary>
        /// Internal Method Called during the loading process.
        /// </summary>
        internal void OnMissingValue(string name, CtpObject value)
        {
            MissingValue(name, value);
        }

        /// <summary>
        /// Internal Method Called during the loading process.
        /// </summary>
        internal void OnMissingElement(string name)
        {
            MissingElement(name);
        }

    }

    /// <summary>
    /// When creating a new command object, the <see cref="T"/> parameter must be the type of the class itself.
    /// The base class then compiles the necessary serialization methods.
    /// Note: The defining type must have a parameterless constructor (it may be private)
    /// </summary>
    /// <typeparam name="T">Must be the value returned by <see cref="Object.GetType"/></typeparam>
    public abstract class CommandObject<T>
        : CommandObject
        where T : CommandObject<T>
    {
        protected CommandObject()
        {
            if (LoadError != null)
                throw LoadError;
            if (typeof(T) != GetType())
                throw new ArgumentException("The supplied type must exactly match the generic type parameter");
        }

        /// <summary>
        /// The name that is associated with this command record. If this is a nested object, this value will be ignored.
        /// </summary>
        public sealed override string CommandName => CmdName;

        public sealed override CtpCommandSchema Schema => WriteSchema;

        public override byte[] ToDataCommandPacket(int schemeRuntimeID)
        {
            T obj = this as T;
            if (LoadError != null)
                throw LoadError;
            var wr = new CtpObjectWriter();
            WriteMethod.Save(obj, wr);
            return PacketMethods.CreatePacket(PacketContents.CommandData, schemeRuntimeID, wr);
        }

        /// <summary>
        /// Converts this object into a <see cref="CtpCommand"/>
        /// </summary>
        /// <returns></returns>
        public sealed override CtpCommand ToCommand()
        {
            T obj = this as T;
            if (LoadError != null)
                throw LoadError;
            var wr = new CtpObjectWriter();
            WriteMethod.Save(obj, wr);
            return new CtpCommand(WriteSchema, wr.ToArray());
        }

        /// <summary>
        /// A method that is used by the defining class to support explicit type casting.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static T FromCommand(CtpCommand command)
        {
            if (LoadError != null)
                throw LoadError;
            if (CmdName != command.RootElement)
                throw new Exception("Document Mismatch");
            var rdr = command.MakeReader();
            rdr.Read();
            return ReadMethod.Load(rdr);
        }

        private static readonly string CmdName;
        private static readonly Exception LoadError;
        private static readonly SerializationWrite.TypeWriteMethodBase<T> WriteMethod;
        private static readonly SerializationRead.TypeReadMethodBase<T> ReadMethod;
        private static readonly CtpCommandSchema WriteSchema;

        static CommandObject()
        {
            try
            {
                SerializationWrite.TypeWrite.Get(out WriteMethod, out var writeSchema, out CmdName);
                WriteSchema = writeSchema.ToSchema();
                SerializationRead.TypeRead<T>.Get(out LoadError, out CmdName, out ReadMethod);
            }
            catch (Exception e)
            {
                LoadError = e;
            }
        }

    }

}
