# coding=utf-8
import os
import time


def input_str(msg):
    while True:
        data = input(msg)
        if data and len(data) > 0:
            return data
        else:
            print("重新输入")
            time.sleep(1000)


def parse_name(name, prefix):
    x = name.lstrip(prefix)
    return x


def print_dirs(dirs):
    for d in dirs:
        print(d)


if __name__ == "__main__":
    prefix = input_str("input prefix:")
    print("prefix:", prefix)

    current_dir = os.getcwd()
    dirs = [x for x in os.listdir(current_dir) if x.startswith(prefix)]
    print("找到相关目录：")
    print_dirs(dirs)

    new_prefix = input_str("input new prefix:")

    dirs = [(x, f"{new_prefix}{parse_name(x,prefix)}") for x in dirs]
    print_dirs(["->".join([x[0].ljust(50, " "), x[1]]) for x in dirs])

    for d in dirs:
        os.rename(os.path.join(current_dir, d[0]), os.path.join(current_dir, d[1]))

    print("finished")
    pass
