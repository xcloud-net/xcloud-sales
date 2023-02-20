import http from '@/utils/http';
import { PlusOutlined } from '@ant-design/icons';
import { Avatar, Button, Card, Input, message, Modal, Table } from 'antd';
import { ColumnType } from 'antd/es/table';
import { useState } from 'react';

const AdminPage = (props: any) => {
  const { show, hide, ok } = props;
  const [loading, _loading] = useState(false);

  const [searchRes, _searchRes] = useState<any>([]);
  const [searchLoading, _searchLoading] = useState(false);

  const [loadingAddId, _loadingAddId] = useState('');

  const queryUserAccount = (q: string) => {
    _searchLoading(true);
    http.adminRequest
      .post('/user/query-user-account', {
        AccountIdentity: q,
      })
      .then((res) => {
        var account = res.data.Data;
        if (account) {
          _searchRes([account]);
        } else {
          _searchRes([]);
          message.info('找不到用户');
        }
      })
      .finally(() => {
        _searchLoading(false);
      });
  };

  const columns: ColumnType<any>[] = [
    {
      title: '头像',
      render: (x) => <Avatar size={'small'} src={x.Avatar} />,
    },
    {
      title: '账号',
      render: (x) => x.IdentityName,
    },
    {
      title: '昵称',
      render: (x) => x.NickName,
    },
    {
      title: '',
      width: 100,
      render: (x) => (
        <Button
          size="small"
          loading={loadingAddId == x.Id}
          icon={<PlusOutlined />}
          type="dashed"
          onClick={() => {
            if (!confirm('确定？')) {
              return;
            }
            _loadingAddId(x.Id);
            http.adminRequest
              .post('/admin/add', {
                UserId: x.Id,
              })
              .then((res) => {
                if (res.data.Error) {
                  message.error(res.data.Error.Message);
                } else {
                  message.success('添加成功');
                  ok && ok();
                }
              })
              .finally(() => {
                _loadingAddId('');
              });
          }}
        >
          设为管理员
        </Button>
      ),
    },
  ];

  return (
    <>
      <Modal
        title="新增用户"
        confirmLoading={loading}
        open={show}
        onCancel={() => {
          hide && hide();
        }}
        footer={false}
        style={{
          padding: 0,
        }}
      >
        <Card
          title="搜索账号"
          size="small"
          extra={
            <Input.Search
              size="small"
              onSearch={(e) => {
                queryUserAccount(e);
              }}
              placeholder="搜索用户名、手机号"
            />
          }
          bordered={false}
        >
          <Table
            loading={searchLoading}
            size="small"
            columns={columns}
            dataSource={searchRes}
            pagination={false}
          />
        </Card>
      </Modal>
    </>
  );
};

export default AdminPage;
