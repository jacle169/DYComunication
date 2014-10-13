package DYCom.J2SE;

import java.io.InputStream;
import java.io.OutputStream;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;

import DYCom.J2SE.dyDelegate;


public class clientPoxy implements Runnable {
    private Socket socket = null;  
    private OutputStream out=null;
    private InputStream in = null;  
    private ByteBuffer buffers=null;
    private int buffersize = 256;
    public dyDelegate dyevent=null;
    
    public clientPoxy(String serverIP,int port, int bufferSize)
    {
    	dyevent=new dyDelegate();
    	buffersize=bufferSize;
    	buffers=ByteBuffer.allocate(buffersize);
    	buffers.order(ByteOrder.LITTLE_ENDIAN);
        try {       	
            socket = new Socket();
			socket.connect(new InetSocketAddress(serverIP, port),10 * 1000);
            out= socket.getOutputStream();
            in = socket.getInputStream();
            new Thread(this).start();
		} catch (Exception e) {
			dis();
		}  
    }
    
	void dis() {
		try {
			canRead = false;
			socket.shutdownInput();
			socket.shutdownOutput();
			socket.close();
		} catch (Exception e) {
		}

		dyevent.DYonconnectPropertyChanged(false);
	}
    

    public int readInt(byte[] bytes) { 
    	int num = bytes[0] & 0xFF; 
    	num |= ((bytes[1] << 8) & 0xFF00); 
    	num |= ((bytes[2] << 16) & 0xFF0000); 
    	num |= ((bytes[3] << 24) & 0xFF000000); 
    	return num; 
    }
    byte[] buf=null;
    boolean canRead=true;
    public void run() {  
        try {  
            while (canRead) {  
                if(socket.isConnected()){  
                    if(!socket.isInputShutdown()){
                      if(in.available() > 0)
                      {
                    	  buf=new byte[in.available()];
                          in.read(buf, 0, buf.length);
                          innerData(buf);
                      }
                     }else{dis();}
                    }else{dis();}  
				Thread.sleep(1);
                }   
        } catch (Exception ex) {  
            dis();
        }  
    }

    void innerData(byte[] buf)
    {
        buffers.put(buf);
        
        buffers.flip();
        byte [] content = new byte[buffers.limit()];
        buffers.get(content);
        
        if(readData(content))
        {
           canRead=false;
           buffers=ByteBuffer.allocate(buffersize);
           buffers.order(ByteOrder.LITTLE_ENDIAN);
           canRead=true;
        	
           ByteBuffer read=ByteBuffer.wrap(content);
           read.order(ByteOrder.LITTLE_ENDIAN);

           int len = read.getInt()-4;
           byte[] m = new byte[len];
           read.get(m);
           
           dyevent.DYondataPropertyChanged(m);
          
        }
    }
    
    boolean readData(byte[] data)
    {
    	ByteBuffer reader=ByteBuffer.wrap(data);
    	reader.order(ByteOrder.LITTLE_ENDIAN);
    	int ml= reader.getInt();
    	if(data.length>=ml)
    	{
    		return true;
    	}else
    	{
    		return false;
    	}
    }
       
    public void send(ByteBuffer msg)
    {
     	try {
            int tempLength = msg.array().length+4;
            ByteBuffer bytebuf = ByteBuffer.allocate(tempLength);
            bytebuf.order(ByteOrder.LITTLE_ENDIAN);
            bytebuf.putInt(tempLength);
            bytebuf.put(msg.array());
			    out.write(bytebuf.array());
				out.flush();
    		} catch (Exception e) {
			dis();
		}
    }
    
 
}

