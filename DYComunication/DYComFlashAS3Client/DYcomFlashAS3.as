package {
	import flash.net.Socket;
	import flash.utils.ByteArray;
	import flash.events.EventDispatcher;
	import flash.events.Event;
	import flash.events.IOErrorEvent;
	import flash.display.MovieClip;
	import DYcom.*;
	/**
	 * ...
	 * @author http://DY2COM.COM
	 */
	public class DYcomFlashAS3 extends MovieClip{
		private var socket:Socket = new Socket();
		private var msgData:ByteArray = new ByteArray();
		private var inData:ByteArray=new ByteArray();
		private var conState:Boolean=false;
		
		public function toJson(obj:Object):String{
			return JSON.encode(obj);
		 }
		 
		public function jsonToClass(jsonString:String):*{
			return JSON.decode(jsonString);
		}
		
		public function get ConnectState():Boolean{
			return conState;
		}

		public function get dyData():ByteArray {
			return inData;
		}
	    public function set dyData(value:ByteArray):void {
			inData=value;
			dispatchEvent(new Event("OnData"));
		}

		public function connet(serverIP:String, port:int):void {
			msgData.endian="littleEndian";
			inData.endian="littleEndian";
			try {
				socket.endian="littleEndian";
				socket.addEventListener("socketData",comedata);
				socket.addEventListener(Event.CONNECT,onConn);
				socket.addEventListener(IOErrorEvent.IO_ERROR, onIOError);
				socket.addEventListener(Event.CLOSE,onclose);
				socket.connect(serverIP,port);
			} catch (e:Error) {
				socket.close();
			}
		}
		
		function onclose(e:Event):void{
			dispatchEvent(new Event("onCloes"));
		}

		function onIOError(e:IOErrorEvent):void{
		   conState=false;
		   dispatchEvent(new Event("OnConnect"));
		}
		
		function onConn(susess):void{
		   conState=susess;
		   dispatchEvent(new Event("OnConnect"));
	    }
		
		public function comedata(event:Event):void {
			var alllen:int=new int();
			alllen=event.target.readInt()-4;
			if (event.target.bytesAvailable>=alllen) {
				var mdt:ByteArray =new ByteArray();
				mdt.endian="littleEndian";
				event.target.readBytes(mdt,0,alllen);
				dyData=mdt;
			}
		}
		
		public function readFloat(msg:ByteArray):Number{
		   return msg.readFloat();
		}
		
		public function readShort(msg:ByteArray):Number{
			return msg.readShort();
		}
		
		public function readByte(msg:ByteArray):int{
			return msg.readByte();
		}
		
		public function readInt(msg:ByteArray):int{
			return msg.readInt();
		}
			
		public function readBool(msg:ByteArray):Boolean{
			return msg.readBoolean();
		}
		
		public function readString(msg:ByteArray):String{
		  return msg.readUTFBytes(msg.readInt());
		}
		
		public function readBytes(msg:ByteArray):ByteArray{
			var len:int = msg.readInt();
			var rdt:ByteArray=new ByteArray();
			rdt.endian="littleEndian";
			msg.readBytes(rdt,rdt.position,len);
			return rdt;
		}
		
		public function readDouble(msg:ByteArray):Number{
			return msg.readDouble();
		}
		
		public function writeByte(msg:int):void{
			if(msg<256)
			{
				msgData.writeByte(msg);
			}
		}
		
		public function writeFloat(msg:Number):void{
			msgData.writeFloat(msg);
		}
		
		public function writeShort(msg:int):void{
			msgData.writeShort(msg);
		}

		public function writeInt(msg:int):void {
			msgData.writeInt(msg);
		}

		public function writeBool(msg:Boolean):void {
			msgData.writeBoolean(msg);
		}

		public function writeString(msg:String):void {
			var mbt:ByteArray =new ByteArray();
			mbt.endian="littleEndian";
			mbt.writeUTFBytes(msg);
			msgData.writeInt(mbt.length);
			msgData.writeBytes(mbt);
		}

		public function writeBytes(msg:ByteArray):void {
			msgData.writeInt(msg.length);
			msgData.writeBytes(msg);
		}

		public function writeDouble(msg:Number):void {
			msgData.writeDouble(msg);
		}

		public function send():void {
			socket.writeInt(msgData.length+4);
			socket.writeBytes(msgData);
			socket.flush();
			msgData.position=0;
		}
		
		public function sendToSmart(mbt:ByteArray):void{
			mbt.writeUTFBytes("\r\n");
			socket.writeInt(mbt.length+4);
			socket.writeBytes(mbt);
			socket.flush();
			mbt.position=0;
		}
	}	

}