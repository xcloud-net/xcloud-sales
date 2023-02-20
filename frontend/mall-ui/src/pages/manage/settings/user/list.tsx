import XTime from '@/components/manage/time';
import XUserAvatar from '@/components/manage/user/avatar';
import u from '@/utils';
import http from '@/utils/http';
import { SysUserDto } from '@/utils/models';
import { PlusOutlined } from '@ant-design/icons';
import { Button, Card, Table, Tag, message } from 'antd';
import { ColumnType } from 'antd/es/table';
import { useEffect, useState } from 'react';
import XAdmin from './adminIdentity';
import XForm from './form';
import XSearchForm from './searchForm';
import XUpdateStatus from './updateStatus';

const AdminPage = (props: any) => {
  const [query, _query] = useState({
    current: 1,
  });
  const [data, _data] = useState([]);
  const [total, _total] = useState(1);
  const [loading, _loading] = useState(false);

  const [showForm, _showForm] = useState(false);

  const queryPaging = () => {
    _loading(true);
    http.adminRequest
      .post('/user/paging', {
        Page: query.current,
      })
      .then((res) => {
        u.handleResponse(res, () => {
          _data(res.data.Items || []);
          _total(res.data.TotalCount);
        });
      })
      .finally(() => {
        _loading(false);
      });
  };

  const updatePwd = (record: any) => {
    var pwd = prompt('请输入新密码，如果放弃输入请留空');
    if (!pwd || u.isEmpty(pwd)) {
      message.info('放弃修改密码');
      return;
    }
    if (pwd.length < 5) {
      message.error('密码长度不能小于5位');
      return;
    }
    http.adminRequest
      .post('/user/set-pwd', {
        Id: record.Id,
        Password: pwd,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          message.success('修改成功');
        }
      });
  };

  const removeMobile = (record: any) => {
    if (!confirm('确定移除手机号？')) {
      return;
    }
    http.adminRequest
      .post('/user/remove-mobiles', {
        Id: record.Id,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          message.success('移除成功');
          queryPaging();
        }
      });
  };

  const updateMobile = (record: any) => {
    var mobile = prompt('请输入手机号码，如果放弃输入请留空');
    if (!mobile || u.isEmpty(mobile)) {
      message.info('放弃设置手机号码');
      return;
    }
    if (!/^(?:(?:\+|00)86)?1[3-9]\d{9}$/.test(mobile)) {
      message.error('手机号码格式不正确');
      return;
    }
    http.adminRequest
      .post('/user/set-mobile', {
        Id: record.Id,
        Mobile: mobile,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          message.success('设置成功');
          queryPaging();
        }
      });
  };

  useEffect(() => {
    queryPaging();
  }, [query]);

  const columns: ColumnType<any>[] = [
    {
      title: '账号主体',
      render: (record: SysUserDto) => <XUserAvatar model={record} />,
    },
    {
      title: '电话',
      render: (record: SysUserDto) => {
        return (
          u.isEmpty(record.AccountMobile) || (
            <Tag
              color={'green'}
              closable
              onClose={() => {
                removeMobile(record);
              }}
            >
              {record.AccountMobile}
            </Tag>
          )
        );
      },
    },
    {
      title: '状态',
      render: (x) => {
        return (
          <XUpdateStatus
            model={x}
            ok={() => {
              queryPaging();
            }}
          />
        );
      },
    },
    {
      title: '管理员身份',
      render: (x) => {
        return (
          <XAdmin
            model={x}
            ok={() => {
              queryPaging();
            }}
          />
        );
      },
    },
    {
      title: '时间',
      render: (x) => <XTime model={x} />,
    },
    {
      title: '操作',
      render: (text, record) => {
        return (
          <Button.Group size="small">
            <Button
              type="dashed"
              onClick={() => {
                updateMobile(record);
              }}
            >
              绑定手机号
            </Button>
            <Button
              type="dashed"
              onClick={() => {
                updatePwd(record);
              }}
            >
              修改密码
            </Button>
          </Button.Group>
        );
      },
    },
  ];
  return (
    <>
      <XForm
        show={showForm}
        hide={() => {
          _showForm(false);
        }}
        ok={() => {
          _showForm(false);
          queryPaging();
        }}
      />
      <XSearchForm
        query={query}
        onSearch={(q: any) => {
          _query(q);
        }}
      />
      <Card
        title="平台用户"
        size="small"
        extra={
          <Button
            size="small"
            type="primary"
            icon={<PlusOutlined />}
            onClick={() => {
              _showForm(true);
            }}
          >
            添加用户
          </Button>
        }
      >
        <Table
          rowKey={(x) => x.Id}
          size="small"
          loading={loading}
          columns={columns}
          dataSource={data}
          pagination={{
            current: query.current,
            total: total,
            pageSize: 20,
            onChange: (page, size) => {
              _query({ ...query, current: page });
            },
          }}
        />
      </Card>
    </>
  );
};

export default AdminPage;
