package DYConJ2SETextClient.test;

import java.nio.ByteBuffer;
import DYCom.J2SE.*;

public class testclient implements dyChangeListener {
	//定义操作符
	static int login=0;
	//DYCom客户端定义
	static clientPoxy poxy;
	//DYcom消息编码器定义
	static DYwriter dm;
	/**
	 * @param args
	 */
	public static void main(String[] args) {
		// TODO Auto-generated method stub
		//实例化DYCom消息编码器
        dm=new DYwriter();
        //实例化DYCom客户端（服务器IP,端口,缓冲区大小）
        poxy=new clientPoxy("218.7.221.181",4510,1024);
        //注册事件管理器
        poxy.dyevent.setDYListener(new testclient());
        
		// json序列化操作
		// JSONObject json=new JSONObject();
		// try {
		//输入数据到json
		// json.put("aa", "jac");
		// json.put("bb", 13);
		// } catch (JSONException e) {
		// TODO Auto-generated catch block
		// e.printStackTrace();
		// }
		// 发送json内容
		// ByteBuffer[] msg ={dm.GetDYBytes(login),dm.GetDYBytes(json.toString(),"UTF-8")};
		// poxy.send(dm.Merge(msg));
        
        // 定义消息准备发送到服务器
        ByteBuffer[] msg ={dm.GetDYBytes(login), dm.GetDYBytes("J2SE: IM J2SE!","UTF-8")};
        // 发送用户输入EditText内容
        poxy.send(dm.Merge(msg));
       
	}
	
	@Override
	public void onError(boolean result) {
		// TODO Auto-generated method stub
		//显示连接结果
		display(Boolean.toString(result));
	}

	@Override
	public void onData(byte[] data) {
		// TODO Auto-generated method stub
		  DYreader read=new DYreader(data);
		  int type =-1;
		  if(!read.ReadInt32())
		  {
			  return;
		  }
		  type=read.Readint32;
		  if(type==login){
			if(read.ReadString("UTF-8"))
			{
				//显示已读的字符串值
			    display(read.Readstring);	
			    
			    //读取其他数据
			    //if(read.ReadString("UTF-8"))
			    //{
				//    display(read.Readstring);	
			    //}
			    
				//json数据读取处理
			    //JSONObject jsonObject;
				//try {
		        //	jsonObject = new JSONObject(read.Readstring);
				//	display(jsonObject.getString("aa"));
				//	display(jsonObject.getString("bb"));
				//} catch (JSONException e) {
					// TODO Auto-generated catch block
				//	e.printStackTrace();
				//}
			}
		   }
	}
	
	void display(String msg)
	{
      System.out.println(msg + "\n");
	}

}
