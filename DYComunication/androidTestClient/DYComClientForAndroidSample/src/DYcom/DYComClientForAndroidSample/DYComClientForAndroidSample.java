package DYcom.DYComClientForAndroidSample;

import java.nio.ByteBuffer;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.TextView;

import DYCom.android.*;

public class DYComClientForAndroidSample extends Activity implements dyChangeListener {
	//定义操作符
	int login=0;
	//DYCom客户端定义
	clientPoxy poxy;
	//DYcom消息编码器定义
	DYwriter dm;
	//消息显示框定义
	TextView tv;
	
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        
        //实例化DYCom消息编码器
        dm=new DYwriter();
        //实例化DYCom客户端（服务器IP,端口,缓冲区大小,连接超时时间，发送超时时间）
        poxy=new clientPoxy("test.dy2com.com",4510,1024,3,5);
        //注册连接事件
        poxy.connectHander.dyevent.setDYListener(this);
        //注册数据到来事件
        poxy.Ondatahandler.dyevent.setDYListener(this);
        
        //添加UI控件
		tv=new TextView(this);
		tv.setHeight(300);
		tv.setWidth(300);
		
		final EditText et=new EditText(this);
		et.setWidth(200);
		Button bt=new Button(this);
		bt.setText("发送消息到dycom服务");
		
		LinearLayout ll=new LinearLayout(this);
		ll.setOrientation(LinearLayout.VERTICAL);  
		ll.addView(tv);
		ll.addView(et);
		ll.addView(bt);
        setContentView(ll);
        
        //按扭点击事件
        bt.setOnClickListener(new View.OnClickListener() {  
			public void onClick(View v) {

				// json序列化操作
				// JSONObject json=new JSONObject();
				// try {
				// 输入数据到json
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
				ByteBuffer[] msg = { dm.GetDYBytes(login),dm.GetDYBytes(et.getText().toString(), "UTF-8") };
				// 发送用户输入EditText内容
				poxy.send(dm.Merge(msg));
			} 
        }); 
    }
    
    //连接事件
    @Override
    public void onError(boolean result)
    {
    	//显示连接结果到UI
    	display(Boolean.toString(result));
    }
    
    //数据到来处理事件
    @Override
	public void onData(byte[] data) {
    	  //实例化DYCOM消息解码器
		  DYreader read=new DYreader(data);
		  //操作符变量
		  int type =-1;
		  //读取消息中的操作符
		  if(!read.ReadInt32())
		  {
			  //如果消息符不合法直接不作任何处理
			  return;
		  }
		  //从解码器中读出消息符
		  type=read.Readint32;
		  //判断操作符
		  if(type==login){
			//尝试读出一个字符串值,使用UTF8解码
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
	
    //显示字符串到UI函数
	void display(String msg)
	{
	  //TextView添加msg字符串
      tv.append(msg + "\n");
	}
    
}