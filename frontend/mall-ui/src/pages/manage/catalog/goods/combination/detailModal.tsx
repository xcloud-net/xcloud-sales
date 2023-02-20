import {Modal, Button} from 'antd';
import {useState} from 'react';
import XDetail from './detail';

export default (props: any) => {
  const {model} = props;
  const [show, _show] = useState(false);

  return <>
    <Modal title='商品明细' width={'90%'} open={show} onCancel={() => {
      _show(false)
    }} footer={false}>
      <XDetail model={model}/>
    </Modal>

    <Button size='small' type='primary' onClick={() => {
      _show(true);
    }}>查看</Button>
  </>
};