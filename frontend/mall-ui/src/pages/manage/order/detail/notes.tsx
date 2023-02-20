import u from '@/utils';
import { OrderDto } from '@/utils/models';
import { Alert, Button, Card, Checkbox, Input, message, Timeline } from 'antd';
import { useEffect, useState } from 'react';

const App = (props: { model: OrderDto }) => {
  const { model } = props;
  const [loading, _loading] = useState(false);
  const [data, _data] = useState<any>([]);

  const [note, _note] = useState('');
  const [DisplayToUser, _DisplayToUser] = useState(false);
  const [loadingSave, _loadingSave] = useState(false);

  const saveNotes = () => {
    if (u.isEmpty(note)) {
      message.error('请输入内容');
      return;
    }
    _loadingSave(true);
    u.http.apiRequest
      .post('/mall-admin/order/add-order-note', {
        OrderId: model.Id,
        DisplayToUser: DisplayToUser,
        Note: note,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          message.success('保存成功');
          _note('');
          _DisplayToUser(false);
          queryNotes();
        });
      })
      .finally(() => {
        _loadingSave(false);
      });
  };

  const queryNotes = () => {
    if (model && model.Id) {
    } else {
      return;
    }
    _loading(true);
    u.http.apiRequest
      .post('/mall-admin/order/list-order-notes', {
        OrderId: model.Id,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          _data(res.data.Data || []);
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  useEffect(() => {
    queryNotes();
  }, [model]);

  return (
    <>
      <Card title="Order Notes" size="small" loading={loading}>
        <div style={{ marginBottom: 10 }}>
          <Input.TextArea
            style={{ marginBottom: 10 }}
            value={note}
            onChange={(e) => {
              _note(e.target.value);
            }}
            maxLength={100}
            placeholder="请输入内容"
          />
          <div
            style={{
              display: 'flex',
              flexDirection: 'row',
              alignItems: 'center',
              justifyContent: 'flex-end',
            }}
          >
            <Checkbox
              checked={DisplayToUser}
              onChange={(e) => {
                _DisplayToUser(e.target.checked);
              }}
              style={{ marginRight: 10 }}
            >
              展示给买家
            </Checkbox>
            <Button
              type="primary"
              loading={loadingSave}
              disabled={u.isEmpty(note)}
              onClick={() => {
                saveNotes();
              }}
            >
              保存
            </Button>
          </div>
        </div>

        {u.isEmpty(data) && <Alert message="无数据"></Alert>}
        {u.isEmpty(data) || (
          <Timeline>
            {u.map(data, (x, index) => {
              return (
                <Timeline.Item color="green" key={index}>
                  <p>{u.dateTimeFromNow(x.CreationTime)}</p>
                  <p>{x.Note}</p>
                </Timeline.Item>
              );
            })}
          </Timeline>
        )}
      </Card>
    </>
  );
};

export default App;
