package dytest.testad;

import java.nio.ByteBuffer;

import android.app.Activity;
import android.view.View;
import android.widget.*;
import android.os.Bundle;
import DYCom.android.*;

public class DYComAndroidTestClient extends Activity implements dyChangeListener {
	int login=0;
	clientPoxy poxy;
	DYwriter dm;
	
	TextView tv;
	
    @Override
    public void onCreate(Bundle savedInstanceState) {   	     
        super.onCreate(savedInstanceState);
               
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
        
        dm=new DYwriter();
        poxy=new clientPoxy("192.168.0.2",4510,1024,3,5);
        poxy.Ondatahandler.dyevent.setDYListener(this);
        poxy.connectHander.dyevent.setDYListener(this);
        
        bt.setOnClickListener(new View.OnClickListener() {  
            public void onClick(View v) {  
            	
            	//json序列化操作
                //JSONObject json=new JSONObject();
                //try {
            	//  输入数据到json
				//	json.put("aa", "jac");
				//	json.put("bb", 13);
				//} catch (JSONException e) {
					// TODO Auto-generated catch block
				//	e.printStackTrace();
				//}
				//发送json内容
                //ByteBuffer[] msg ={dm.GetDYBytes(login),dm.GetDYBytes(json.toString(),"UTF-8")};
                //      poxy.send(dm.Merge(msg));
                      
                //发送用户输入EditText内容
                ByteBuffer[] msg ={dm.GetDYBytes(login),dm.GetDYBytes(et.getText().toString(),"UTF-8")};
                poxy.send(dm.Merge(msg));
            }  
        }); 

    }
    
    public void onError(boolean result)
    {
    	display(Boolean.toString(result));
    }

	@Override
	public void onData(byte[] data) {
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
      tv.append(msg + "\n");
	}
    
}