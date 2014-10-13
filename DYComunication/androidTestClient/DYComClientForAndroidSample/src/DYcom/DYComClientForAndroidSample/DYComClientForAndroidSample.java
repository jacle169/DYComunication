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
	//���������
	int login=0;
	//DYCom�ͻ��˶���
	clientPoxy poxy;
	//DYcom��Ϣ����������
	DYwriter dm;
	//��Ϣ��ʾ����
	TextView tv;
	
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        
        //ʵ����DYCom��Ϣ������
        dm=new DYwriter();
        //ʵ����DYCom�ͻ��ˣ�������IP,�˿�,��������С,���ӳ�ʱʱ�䣬���ͳ�ʱʱ�䣩
        poxy=new clientPoxy("test.dy2com.com",4510,1024,3,5);
        //ע�������¼�
        poxy.connectHander.dyevent.setDYListener(this);
        //ע�����ݵ����¼�
        poxy.Ondatahandler.dyevent.setDYListener(this);
        
        //���UI�ؼ�
		tv=new TextView(this);
		tv.setHeight(300);
		tv.setWidth(300);
		
		final EditText et=new EditText(this);
		et.setWidth(200);
		Button bt=new Button(this);
		bt.setText("������Ϣ��dycom����");
		
		LinearLayout ll=new LinearLayout(this);
		ll.setOrientation(LinearLayout.VERTICAL);  
		ll.addView(tv);
		ll.addView(et);
		ll.addView(bt);
        setContentView(ll);
        
        //��Ť����¼�
        bt.setOnClickListener(new View.OnClickListener() {  
			public void onClick(View v) {

				// json���л�����
				// JSONObject json=new JSONObject();
				// try {
				// �������ݵ�json
				// json.put("aa", "jac");
				// json.put("bb", 13);
				// } catch (JSONException e) {
				// TODO Auto-generated catch block
				// e.printStackTrace();
				// }
				// ����json����
				// ByteBuffer[] msg ={dm.GetDYBytes(login),dm.GetDYBytes(json.toString(),"UTF-8")};
				// poxy.send(dm.Merge(msg));

				// ������Ϣ׼�����͵�������
				ByteBuffer[] msg = { dm.GetDYBytes(login),dm.GetDYBytes(et.getText().toString(), "UTF-8") };
				// �����û�����EditText����
				poxy.send(dm.Merge(msg));
			} 
        }); 
    }
    
    //�����¼�
    @Override
    public void onError(boolean result)
    {
    	//��ʾ���ӽ����UI
    	display(Boolean.toString(result));
    }
    
    //���ݵ��������¼�
    @Override
	public void onData(byte[] data) {
    	  //ʵ����DYCOM��Ϣ������
		  DYreader read=new DYreader(data);
		  //����������
		  int type =-1;
		  //��ȡ��Ϣ�еĲ�����
		  if(!read.ReadInt32())
		  {
			  //�����Ϣ�����Ϸ�ֱ�Ӳ����κδ���
			  return;
		  }
		  //�ӽ������ж�����Ϣ��
		  type=read.Readint32;
		  //�жϲ�����
		  if(type==login){
			//���Զ���һ���ַ���ֵ,ʹ��UTF8����
			if(read.ReadString("UTF-8"))
			{
				//��ʾ�Ѷ����ַ���ֵ
			    display(read.Readstring);	
			    
			    //��ȡ��������
			    //if(read.ReadString("UTF-8"))
			    //{
				//    display(read.Readstring);	
			    //}
			    
				//json���ݶ�ȡ����
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
	
    //��ʾ�ַ�����UI����
	void display(String msg)
	{
	  //TextView���msg�ַ���
      tv.append(msg + "\n");
	}
    
}