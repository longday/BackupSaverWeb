import { ILog } from "./components/LogTable";

export class Log implements ILog
{
    public date: Date;
    public message : string;

    public constructor(date: Date, message: string)
    {
        this.date = date;
        this.message = message;
    }
}